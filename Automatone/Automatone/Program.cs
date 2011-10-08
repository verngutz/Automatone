using System;
using System.IO;

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
            if (AppDomain.CurrentDomain.SetupInformation.ActivationArguments != null
                && AppDomain.CurrentDomain.SetupInformation.ActivationArguments.ActivationData != null
                && AppDomain.CurrentDomain.SetupInformation.ActivationArguments.ActivationData.Length > 0)
            {
                Automatone.Instance.ProjectStream = new FileStream(AppDomain.CurrentDomain.SetupInformation.ActivationArguments.ActivationData[0], FileMode.Open);
            }
            Automatone.Instance.Run();
        }
    }
#endif
}

