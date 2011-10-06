using System;

namespace Automatone
{
    [Serializable]
    public class InputParameters
    {
        //Global Song Parameters
        public ushort Tempo { set; get; }
        public int TimeSignatureN { set; get; }
        public int TimeSignatureD { set; get; }
        public double TimeSignature { get { return (TimeSignatureN / (double)TimeSignatureD); } }
        
        //Song Parameters
        public ParameterWrapper<SongParameter> MeanSongLength { set; get; }
        public ParameterWrapper<SongParameter> StructuralVariance { set; get; }
        public ParameterWrapper<SongParameter> SongRhythmVariance { set; get; }
        public ParameterWrapper<SongParameter> SongMelodyVariance { set; get; }
        public ParameterWrapper<SongParameter> SongLengthVariance { set; get; }

        //Verse Parameters
        public ParameterWrapper<VerseParameter> MeanVerseLength { set; get; }
        public ParameterWrapper<VerseParameter> VerseLengthVariance { set; get; }
        public ParameterWrapper<VerseParameter> VerseRhythmVariance { set; get; }
        public ParameterWrapper<VerseParameter> VerseMelodyVariance { set; get; }

        //Phrase Parameters
        public ParameterWrapper<PhraseParameter> MeanPhraseLength { set; get; }
        public ParameterWrapper<PhraseParameter> PhraseLengthVariance { set; get; }
        public ParameterWrapper<PhraseParameter> PhraseRhythmVariance { set; get; }
        public ParameterWrapper<PhraseParameter> PhraseMelodyVariance { set; get; }

        //Measure Parameters
        public ParameterWrapper<MeasureParameter> MeasureRhythmVariance { set; get; }
        public ParameterWrapper<MeasureParameter> MeasureMelodyVariance { set; get; }

        //Part Parameters
        public ParameterWrapper<PartParameter> Homophony { set; get; }
        public ParameterWrapper<PartParameter> Polyphony { set; get; }
        public ParameterWrapper<PartParameter> BeatDefinition { set; get; }

        //Per-part Parameters
        public ParameterWrapper<PerPartParameter> MeanPartRhythmCrowdedness { set; get; }
        public ParameterWrapper<PerPartParameter> PartRhythmCrowdednessVariance { set; get; }
        public ParameterWrapper<PerPartParameter> PartNoteLengthVariance { set; get; }
        public ParameterWrapper<PerPartParameter> MeanPartOctaveRange { set; get; }
        public ParameterWrapper<PerPartParameter> PartOctaveRangeVariance { set; get; }

        //Note Parameters
        public ParameterWrapper<NoteParameter> MeanNoteLength { set; get; }
        public ParameterWrapper<NoteParameter> NoteLengthVariance { set; get; }

        //Rhythm
        public ParameterWrapper<RhythmParameter> RhythmObedience { set; get; }
        
        //Melody
        public ParameterWrapper<MelodyParameter> ChordalityObedience { set; get; }
        public ParameterWrapper<MelodyParameter> TonalityObedience { set; get; }
        public ParameterWrapper<MelodyParameter> MeanPitchContiguity { set; get; }

        //Harmony
        public ParameterWrapper<HarmonyParameter> SeventhChordProbability { set; get; }
        public ParameterWrapper<HarmonyParameter> Mood { set; get; }

        private InputParameters() { }

        public void Initialize()
        {
            Tempo = 60;
            TimeSignatureN = 4;
            TimeSignatureD = 4;
            
            //Song Parameters
            MeanSongLength = 0.5;
            StructuralVariance = 0.5;
            SongRhythmVariance = 0.5;
            SongMelodyVariance = 0.5;
            SongLengthVariance = 0.5;

            //Verse Parameters
            MeanVerseLength = 0.5;
            VerseLengthVariance = 0.5;
            VerseRhythmVariance = 0.5;
            VerseMelodyVariance = 0.5;

            //Phrase Parameters
            MeanPhraseLength = 0.5;
            PhraseLengthVariance = 0.5;
            PhraseRhythmVariance = 0.5;
            PhraseMelodyVariance = 0.5;

            //Measure Parameters
            MeasureRhythmVariance = 0.5;
            MeasureMelodyVariance = 0.5;

            //Part Parameters
            Homophony = 0.5;
            Polyphony = 0.5;
            BeatDefinition = 0.5;
            //Per-part Parameters
            MeanPartRhythmCrowdedness = 0.5;
            PartRhythmCrowdednessVariance = 0.5;
            PartNoteLengthVariance = 0.5;
            MeanPartOctaveRange = 1;
            PartOctaveRangeVariance = 1;

            //Note Parameters
            MeanNoteLength = 0.5;
            NoteLengthVariance = 0.5;

            //Rhythm
            RhythmObedience = 0.9;
        
            //Melody
            ChordalityObedience = 0.9;
            TonalityObedience = 0.9;
            MeanPitchContiguity = 0.5;

            //Harmony
            SeventhChordProbability = 0.1;
            Mood = 0.5;
        }

        private static InputParameters instance = null;

        public static InputParameters Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new InputParameters();
                }
                return instance;
            }
        }

        public static void LoadInstance(InputParameters instance)
        {
            InputParameters.instance = instance;
        }
    }

    // Used for reflection
    public interface ZeroOneParameter { }

    [Serializable]
    public struct ParameterWrapper<T> where T : ZeroOneParameter
    {
        private double value;
        public double Value { get { return value; } }
        public ParameterWrapper(double value)
        {
            this.value = value;
        }

        public static implicit operator ParameterWrapper<T>(double value)
        {
            return new ParameterWrapper<T>(value);
        }

        public static implicit operator double(ParameterWrapper<T> parameter)
        {
            return parameter.value;
        }
    }

    public static class ParameterWrapperFactory
    {
        public static string WrapperMethodName { get { return "MakeWrapper"; } }
        public static string DoubleMethodName { get { return "MakeDouble"; } }

        public static object MakeWrapper<T>(double value) where T : ZeroOneParameter
        {
            return new ParameterWrapper<T>(value);
        }

        public static object MakeDouble<T>(ParameterWrapper<T> parameter) where T : ZeroOneParameter
        {
            return parameter.Value;
        }
    }

    public interface SongParameter : ZeroOneParameter { }
    public interface VerseParameter : ZeroOneParameter { }
    public interface PhraseParameter : ZeroOneParameter { }
    public interface MeasureParameter : ZeroOneParameter { }
    public interface PartParameter : ZeroOneParameter { }
    public interface PerPartParameter : ZeroOneParameter { }
    public interface NoteParameter : ZeroOneParameter { }
    public interface RhythmParameter : ZeroOneParameter { }
    public interface MelodyParameter : ZeroOneParameter { }
    public interface HarmonyParameter : ZeroOneParameter { }
}
