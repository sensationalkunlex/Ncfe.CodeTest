using System.Configuration;
using System;
using Unity;
using Unity.Injection;

namespace Ncfe.CodeTest.IOC
{
    public static class UnityConfiguration
    {
        public static IUnityContainer RegisterComponents()
        {
            var container = new UnityContainer();
            container.RegisterType<ILearnerDataAccess, LearnerDataAccess>();
            container.RegisterType<IFailoverRepository, FailoverRepository>();
            container.RegisterType<IArchivedDataService, ArchivedDataService>();
            container.RegisterType<IFailoverRepository, IFailoverRepository>();
            /// appConfig ConfigurationManager.AppSettings["FailoverPeriodInMinutes"])
            ///  ConfigurationManager.AppSettings["FailoverLimit"]);
            int failoverPeriodInMinutes = 30;
            int failoverLimit = 10; 
            TimeSpan failoverPeriod = TimeSpan.FromMinutes(failoverPeriodInMinutes);
            container.RegisterType<IFailoverReviewService, FailoverReviewService>
                (new InjectionConstructor(failoverPeriod, failoverLimit));

            return container;
        }
    }
}
