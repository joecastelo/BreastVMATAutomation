using BreastVMATAutomation.Model;
using BreastVMATAutomation.Model.ESAPI;
using BreastVMATAutomation.Model.Templating;
using System;
using System.Collections.Generic;
using VMS.TPS.Common.Model.API;
using System.Linq;
using VMS.TPS.Common.Model.Types;

namespace BreastVMATAutomation.UI
{
    internal class RightBreastGeometryTool : IBreastGeometryTool
    {
        private ExternalPlanSetup Plan;
        private IEnumerable<Structure> SelectedTargets;
        private ArcModel SelectedSetup;
        public NonRDSOptions NonRDSOptions { get; set; }
        VVector Isocenter;
        ExternalBeamMachineParameters Parameters;
        public bool RDS;

        public Structure Target;
        float [,] mlc_leaf_pos { get => RDS ? new float[2, 57] : new float[2, 60]; }
        public RightBreastGeometryTool(ExternalPlanSetup plan, IEnumerable<Structure> selectedTargets,
            ArcModel selectedSetup, NonRDSOptions nonRDSOptions)
        {
            Plan = plan;
            SelectedTargets = selectedTargets;
            SelectedSetup = selectedSetup;
            NonRDSOptions = nonRDSOptions;
            RDS = Plan.Beams.First().TreatmentUnit.MachineModelName.ToUpper().Contains("RDS");
            Plan.Course.Patient.BeginModifications();
            Target = SelectedTargets.First();
            if (selectedTargets.Count() > 1)
            {
                try
                {
                    var nTarget= Plan.StructureSet.AddStructure("CONTROL", "z_targetSumRun");
                    nTarget.SegmentVolume = Target.SegmentVolume;
                    foreach (var structure in selectedTargets)
                    {
                        nTarget.Or(structure);
                    }
                    Target = nTarget;
                }
                catch (Exception)
                {

                    
                }
            }

            CalculationModels.PrepareCalcOptions(Plan);
        }
        
        public void RemoveCreatedStructures()
        {
            if (SelectedTargets.Count() > 1)
            {
                try
                {
                    Plan.StructureSet.RemoveStructure(Target);

                }
                catch (Exception)
                {

                    
                }
            }
        }
        public void ChangeIsocenter() // 2 isocenters still have to be developed! 
        {
            Isocenter = SelectIsocenter.ChangeIsocenter(Plan, Target, false);

        }


        private double[] MeterSetWeight()
        {
            double[] metersetWeight = new double[101];

            for (int i = 0; i < 101; i++)
            {
                metersetWeight[i] = (1.0 / 100.0) * i;
            }
            return metersetWeight;
        }
        public void CreateArcsBasedOnModel()
        {

            try
            {
                Parameters = GetParameters();
                var bestInner = CalculateBeamBestInner();
                var bestOuter = CalculateBeamBestOuter();
                try
                {
                    Plan.Beams.Where(e => !e.IsSetupField).ToList().ForEach(Z => Plan.RemoveBeam(Z));

                }
                catch (Exception)
                {

                    Console.WriteLine("No beams to delete");
                }



                foreach (var wise in Enumerable.Range(0, SelectedSetup.NbPartialArcs / 2))
                {
                    if (wise % 2 == 0)
                    {
                        var inner = GetInnerBeam(bestInner, 360 - (wise + 1) * 5);
                        var outer = GetOuterBeam(350, bestOuter, 5 * (1 + wise));

                    }
                    else
                    {
                        var outer = GetOuterBeam(10, bestOuter, 5 * (1 + wise));
                        ReverseArc(outer);
                        Plan.RemoveBeam(outer);
                        var inner = GetInnerBeam(bestInner, 360 - (wise + 1) * 5);
                        ReverseArc(inner);
                        Plan.RemoveBeam(inner);

                    }
                }
                if (NonRDSOptions.IncludeNodes)
                {
                    var node = GetNodeArc();
                }
            }
            catch (Exception e)
            {

                Console.WriteLine(e.Message + "\n" +  e.StackTrace);
                Console.ReadLine();
            }


        }


        private Beam GetNodeArc()
        {
            var machineParameters = Parameters;
            var mlcBeam = Plan.AddMLCArcBeam(machineParameters, mlc_leaf_pos, new VRect<double>(), 95, 40, 181, GantryDirection.CounterClockwise, 0, Isocenter);
            mlcBeam.FitCollimatorToStructure(new FitToStructureMargins(10, 10, 10, 10), Target, true, true, false);
            var edit = mlcBeam.GetEditableParameters();
            var jawPos = edit.ControlPoints.First().JawPositions;
            edit.SetJawPositions(new VRect<double>(0, jawPos.Y1, jawPos.X2 > 150 ? 150 : jawPos.X2, jawPos.Y2 ));
            mlcBeam.ApplyParameters(edit);
            if (SelectedSetup.NbPartialArcs%4 != 0)
            {
                var oth = ReverseArc(mlcBeam);
                Plan.RemoveBeam(mlcBeam);
                return oth;

            }
            return mlcBeam;
        }
        private Beam ReverseArc(Beam beam)
        {
            var machineParameters = Parameters;
            var isocenter = Isocenter;
            var initAngle = beam.ControlPoints.First().GantryAngle;
            var finalAngle = beam.ControlPoints.Last().GantryAngle;
            var colAngle = beam.ControlPoints.First().CollimatorAngle;
            var editparams = beam.GetEditableParameters();
            var jawPost = editparams.ControlPoints.First().JawPositions;
            if (RDS)
            {
                return Plan.AddVMATBeamForFixedJaws(machineParameters, MeterSetWeight(), colAngle, finalAngle, initAngle, GantryDirection.Clockwise, 0, isocenter);
            }
            return Plan.AddMLCArcBeam(machineParameters, mlc_leaf_pos, jawPost, colAngle, finalAngle,
                initAngle, GantryDirection.CounterClockwise, 0, isocenter);
        }
        private int CalculateBeamBestInner()
        {
            var innerAnglesAreas = new Dictionary<int,double>();
            if (RDS)
            {
                return 50 + SelectedSetup.InnerAngleMargin;
            }
            try
            {
                for (int i = 0; i < 12; i++)
                {
                    var initAngle = 30;
                    ExternalBeamMachineParameters parameters = GetStaticParameters();
                    var beam = Plan.AddStaticBeam(parameters, new VRect<double>(), 0, initAngle + (i * 5), 0, Isocenter);
                    beam.FitCollimatorToStructure(new FitToStructureMargins(35, 7, 0, 7), Target, true, true, false);
                    if (NonRDSOptions.AvoidEntryCLBreast)
                    {
                        var outLine = beam.GetStructureOutlines(NonRDSOptions.BreastContralateral, false);
                        var countPoints = BeamParameterCalculations.ConvertJaggedPointArrayToPointList(outLine);
                        var npoints = BeamParameterCalculations.PointsInsideJawRight(countPoints, beam);
                        if (npoints < 20)
                        {
                            var body = Plan.StructureSet.Structures.First(E => E.DicomType == "EXTERNAL");
                            innerAnglesAreas.Add(initAngle + (i * 5),
                                BeamParameterCalculations.PercentageAreaComparingBody(beam, body, Target));
                        }
                        Plan.RemoveBeam(beam);

                    }
                    else
                    {

                        var body = Plan.StructureSet.Structures.First(E => E.DicomType == "EXTERNAL");
                        innerAnglesAreas.Add(initAngle + (i * 5),
                            BeamParameterCalculations.PercentageAreaComparingBody(beam, body, Target));
                        Plan.RemoveBeam(beam);
                    }
                }

                return innerAnglesAreas.OrderBy(e => e.Value).First().Key + SelectedSetup.InnerAngleMargin;
            }
            catch (Exception)
            {

                return 50 + SelectedSetup.InnerAngleMargin;

            }

        }
        private int CalculateBeamBestOuter()
        {
            var innerAnglesAreas = new Dictionary<int, double>();
            if (RDS)
            {
                return 220;
            }
            try
            {
                for (int i = 0; i < 10; i++)
                {
                    var initAngle = 270;
                    ExternalBeamMachineParameters parameters = GetStaticParameters();
                    var beam = Plan.AddStaticBeam(parameters, new VRect<double>(), 0, initAngle - (i * 5), 0, Isocenter);
                    beam.FitCollimatorToStructure(new FitToStructureMargins(0, 7, 35, 7), Target, true, true, false);



                    var body = Plan.StructureSet.Structures.First(E => E.DicomType == "EXTERNAL");
                    innerAnglesAreas.Add(initAngle + (i * 5),
                        BeamParameterCalculations.PercentageAreaComparingBody(beam, body, Target));
                    Plan.RemoveBeam(beam);
                    
                }

                return innerAnglesAreas.OrderBy(e => e.Value).First().Key - SelectedSetup.InnerAngleMargin;
            }
            catch (Exception)
            {

                return 220;

            }

        }



        private Beam GetOuterBeam(int innerLastAngle, int outerGantryAngle,int collimatorAngle)
        {
            if (RDS)
            {
                return GetOuterBeamRDS(innerLastAngle, outerGantryAngle, collimatorAngle);
            }
            var machineParameters = Parameters;
            var isocenter = Isocenter;
            var mlcBeam = Plan.AddMLCArcBeam(machineParameters, mlc_leaf_pos, new VRect<double>(), collimatorAngle, outerGantryAngle, outerGantryAngle+10, GantryDirection.CounterClockwise, 0, isocenter);
            mlcBeam.FitCollimatorToStructure(new FitToStructureMargins(0, 7, 35, 7), Target, true, true, false);
            var editparams = mlcBeam.GetEditableParameters();
            var jawPost = editparams.ControlPoints.First().JawPositions;
            Plan.RemoveBeam(mlcBeam);
            var finalAngle = outerGantryAngle - SelectedSetup.OuterAngleMargin < 181 ? 181 : outerGantryAngle - SelectedSetup.OuterAngleMargin;
            mlcBeam = Plan.AddMLCArcBeam(machineParameters, mlc_leaf_pos, jawPost, collimatorAngle, innerLastAngle - SelectedSetup.GapSectorAngle, 
                finalAngle, GantryDirection.CounterClockwise, 0, isocenter);
            if (NonRDSOptions.XLessThan15)
            {
                var jawSum = BeamParameterCalculations.GetXColimatorSize(mlcBeam);
                if (jawSum > 150)
                {
                    mlcBeam = LimitJawSizeX1(mlcBeam);
                }
            }
            return mlcBeam;
        }
        private Beam GetInnerBeam(int gantryAngle, int collimatorAngle)
        {
            if (RDS)
            {
                return GetInnerBeamRDS(gantryAngle, collimatorAngle);
            }
            var machineParameters = Parameters;
            var mlcBeam = Plan.AddMLCArcBeam(machineParameters, mlc_leaf_pos, new VRect<double>(), collimatorAngle, gantryAngle, 350, GantryDirection.CounterClockwise, 0, Isocenter);
            mlcBeam.FitCollimatorToStructure(new FitToStructureMargins(35, 7, 0, 7), Target, true, true, false);
            if (NonRDSOptions.XLessThan15)
            {
                var jawSum = BeamParameterCalculations.GetXColimatorSize(mlcBeam);
                if (jawSum > 150)
                {
                    mlcBeam = LimitJawSizeX1(mlcBeam);
                }
            }
            return mlcBeam;
        }
        private Beam LimitJawSizeX2(Beam mlcBeam)
        {
            var jawSum = BeamParameterCalculations.GetXColimatorSize(mlcBeam);
            var edit = mlcBeam.GetEditableParameters();
            var jawPos = edit.ControlPoints.First().JawPositions;
            edit.SetJawPositions(new VRect<double>(jawPos.X1, jawPos.Y1, jawPos.X2 - jawSum + 150  , jawPos.Y2));
            mlcBeam.ApplyParameters(edit);
            return mlcBeam;
        }
        private Beam LimitJawSizeX1(Beam mlcBeam)
        {
            var jawSum = BeamParameterCalculations.GetXColimatorSize(mlcBeam);

            var edit = mlcBeam.GetEditableParameters();
            var jawPos = edit.ControlPoints.First().JawPositions;
            edit.SetJawPositions(new VRect<double>(jawPos.X1 + jawSum - 150 , jawPos.Y1, jawPos.X2, jawPos.Y2));
            mlcBeam.ApplyParameters(edit);
            return mlcBeam;
        }
        private Beam GetInnerBeamRDS(int gantryAngle, int collimatorAngle)
        {
            var machineParameters = Parameters;
            var mlcBeam = Plan.AddVMATBeamForFixedJaws(machineParameters, MeterSetWeight() ,  collimatorAngle, gantryAngle,
                350, GantryDirection.CounterClockwise, 0, Isocenter);
            return mlcBeam;
        }
        private Beam GetOuterBeamRDS(int innerLastAngle, int outerGantryAngle, int collimatorAngle)
        {
            var machineParameters = Parameters;
            var finalAngle = outerGantryAngle -SelectedSetup.OuterAngleMargin < 181 ? 181 : outerGantryAngle - SelectedSetup.OuterAngleMargin;

            var mlcBeam = Plan.AddVMATBeamForFixedJaws(machineParameters, MeterSetWeight(),
                collimatorAngle,  innerLastAngle - SelectedSetup.GapSectorAngle,
                finalAngle, GantryDirection.CounterClockwise, 0, Isocenter);
            return mlcBeam;
        }
        private ExternalBeamMachineParameters GetParameters()
        {
            var fBeam = Plan.Beams.First(e => !e.IsSetupField);
            var enid = fBeam.EnergyModeDisplayName.Contains("-") ?
                 fBeam.EnergyModeDisplayName.Split('-').First() : fBeam.EnergyModeDisplayName;
            var pflu = fBeam.EnergyModeDisplayName.Contains("-") ?
                 fBeam.EnergyModeDisplayName.Split('-').Last() : string.Empty;
            ExternalBeamMachineParameters machineParameters = new ExternalBeamMachineParameters(fBeam.TreatmentUnit.Id, enid, fBeam.DoseRate, "ARC", pflu);
            return machineParameters;
        }
        private ExternalBeamMachineParameters GetStaticParameters()
        {
            var fBeam = Plan.Beams.First(e => !e.IsSetupField);
            var enid = fBeam.EnergyModeDisplayName.Contains("-") ?
                 fBeam.EnergyModeDisplayName.Split('-').First() : fBeam.EnergyModeDisplayName;
            var pflu = fBeam.EnergyModeDisplayName.Contains("-") ?
                 fBeam.EnergyModeDisplayName.Split('-').Last() : string.Empty;
            ExternalBeamMachineParameters machineParameters = new ExternalBeamMachineParameters(fBeam.TreatmentUnit.Id, enid, fBeam.DoseRate, "STATIC", pflu);
            return machineParameters;
        }
    }
}