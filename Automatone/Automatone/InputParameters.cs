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
        public MusicParameter<SongParameter> MeanSongLength { set; get; }
        public MusicParameter<SongParameter> StructuralVariance { set; get; }
        public MusicParameter<SongParameter> SongRhythmVariance { set; get; }
        public MusicParameter<SongParameter> SongMelodyVariance { set; get; }
        public MusicParameter<SongParameter> SongLengthVariance { set; get; }

        //Verse Parameters
        public MusicParameter<VerseParameter> MeanVerseLength { set; get; }
        public MusicParameter<VerseParameter> VerseLengthVariance { set; get; }
        public MusicParameter<VerseParameter> VerseRhythmVariance { set; get; }
        public MusicParameter<VerseParameter> VerseMelodyVariance { set; get; }

        //Phrase Parameters
        public MusicParameter<PhraseParameter> MeanPhraseLength { set; get; }
        public MusicParameter<PhraseParameter> PhraseLengthVariance { set; get; }
        public MusicParameter<PhraseParameter> PhraseRhythmVariance { set; get; }
        public MusicParameter<PhraseParameter> PhraseMelodyVariance { set; get; }

        //Measure Parameters
        public MusicParameter<MeasureParameter> MeasureRhythmVariance { set; get; }
        public MusicParameter<MeasureParameter> MeasureMelodyVariance { set; get; }

        //Part Parameters
        public MusicParameter<PartParameter> Homophony { set; get; }
        public MusicParameter<PartParameter> Polyphony { set; get; }
        public MusicParameter<PartParameter> BeatDefinition { set; get; }

        //Per-part Parameters
        public MusicParameter<PerPartParameter> MeanPartRhythmCrowdedness { set; get; }
        public MusicParameter<PerPartParameter> PartRhythmCrowdednessVariance { set; get; }
        public MusicParameter<PerPartParameter> PartNoteLengthVariance { set; get; }
        public MusicParameter<PerPartParameter> MeanPartOctaveRange { set; get; }
        public MusicParameter<PerPartParameter> PartOctaveRangeVariance { set; get; }

        //Note Parameters
        public MusicParameter<NoteParameter> MeanNoteLength { set; get; }
        public MusicParameter<NoteParameter> NoteLengthVariance { set; get; }

        //Rhythm
        public MusicParameter<RhythmParameter> RhythmObedience { set; get; }
        
        //Melody
        public MusicParameter<MelodyParameter> ChordalityObedience { set; get; }
        public MusicParameter<MelodyParameter> TonalityObedience { set; get; }
        public MusicParameter<MelodyParameter> MeanPitchContiguity { set; get; }

        //Harmony
        public MusicParameter<HarmonyParameter> SeventhChordProbability { set; get; }

        private InputParameters() 
        {
            Tempo = 120;
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
    public interface MusicParameter { }
    public struct MusicParameter<T> where T : MusicParameter
    {
        private double value;
        private MusicParameter(double value)
        {
            this.value = value;
        }

        public static implicit operator MusicParameter<T>(double value)
        {
            return new MusicParameter<T>(value);
        }

        public static implicit operator double(MusicParameter<T> parameter)
        {
            return parameter.value;
        }
    }

    public interface SongParameter : MusicParameter { }
    public interface VerseParameter : MusicParameter { }
    public interface PhraseParameter : MusicParameter { }
    public interface MeasureParameter : MusicParameter { }
    public interface PartParameter : MusicParameter { }
    public interface PerPartParameter : MusicParameter { }
    public interface NoteParameter : MusicParameter { }
    public interface RhythmParameter : MusicParameter { }
    public interface MelodyParameter : MusicParameter { }
    public interface HarmonyParameter : MusicParameter { }
}
