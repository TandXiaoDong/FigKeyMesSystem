using SwaggerWcf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Activation;
using System.Web;
using System.Web.Routing;
using CommonUtils.DB;
using System.Configuration;

namespace MesAPI
{
    public class Global : System.Web.HttpApplication
    {

        protected void Application_Start(object sender, EventArgs e)
        {
            SQLServer.SqlConnectionString = ConfigurationManager.ConnectionStrings["ConnectionString"].ToString();
            RouteTable.Routes.Add(new ServiceRoute("MesService", new WebServiceHostFactory(), typeof(MesService)));
            RouteTable.Routes.Add(new ServiceRoute("api-docs", new WebServiceHostFactory(), typeof(SwaggerWcfEndpoint)));
        }

        protected void Session_Start(object sender, EventArgs e)
        {

        }

        protected void Application_BeginRequest(object sender, EventArgs e)
        {

        }

        protected void Application_AuthenticateRequest(object sender, EventArgs e)
        {

        }

        protected void Application_Error(object sender, EventArgs e)
        {

        }

        protected void Session_End(object sender, EventArgs e)
        {

        }

        protected void Application_End(object sender, EventArgs e)
        {

        }
    }
}