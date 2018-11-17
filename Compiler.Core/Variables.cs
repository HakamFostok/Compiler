namespace Compiler.Core
{
    class Variables
    {
        internal TDefine gdefine;
        internal TDefine Gdefine
        {
            get
            {
                return gdefine;
            }
            set
            {
                gdefine = value;
            }
        }

        internal TVar gvar;
        internal TVar Gvar
        {
            get
            {
                return gvar;
            }
            set
            {
                gvar = value;
            }
        }

        internal TProcedure gproc;
        internal TProcedure GPorc
        {
            get
            {
                return gproc;
            }
            set
            {
                gproc = value;
            }
        }

        internal TFile Gfile { get; set; }
        internal TFile CurrFile { get; set; }
        internal TProcedure CurrPorc { get; set; }
        internal int lineNumber { get; set; }
        internal int CI { get; set; }
        internal string[] CF { get; set; }
        internal string CL { get; set; }
        internal double G_curr_NB { get; set; }
        internal string G_curr_str { get; set; }
        internal TIdentifier G_curr_id { get; set; }

        internal void initializeForRecompile()
        {
            initializeForNewFile();
            CurrFile = null;
            Gfile = null;
            GPorc = null;
        }

        internal void initializeForNewFile()
        {
            CurrPorc = null;
            Gvar = null;
            Gdefine = null;

            lineNumber = 0;
            CI = 0;
            CL = null;

            G_curr_NB = 0.0;
            G_curr_str = null;
            G_curr_id = null;
            CF = null;
        }
    }
}