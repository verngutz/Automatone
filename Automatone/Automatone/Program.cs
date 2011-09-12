using System;

namespace Automatone
{
#if WINDOWS || XBOX
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        /// 
        [STAThread]
        static void Main(string[] args)
        {
            using (Automatone game = new Automatone())
            {
                game.Run();
            }
        }
    }
#endif
}

