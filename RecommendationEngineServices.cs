using RecommendationEngine.Quartz;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace RecommendationEngine
{
    public partial class RecommendationEngineServices : ServiceBase
    {
        public RecommendationEngineServices()
        {
            InitializeComponent();
        }
        protected override void OnStart(string[] args)
        {
            SchedularJob.Start();
        }
        protected override void OnStop()
        {

        }
    }
}
