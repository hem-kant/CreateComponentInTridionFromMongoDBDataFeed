using MongoDBToTridion.BAL.CoreServiceFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDBToTridion.BAL;
using MongoDBToTridion.BAL.Model;
using System.Configuration;
using System.Net;
using MongoDBToTridion.BAL.Helper;
using Tridion.ContentManager.CoreService.Client;

namespace MongoDBToTridion.BAL
{
    public class Generation
    {
        public static ICoreServiceFrameworkContext coreService = null;
        public static void process(List<Article> DataFrommongo)
        {
            List<Article> article = DataFrommongo;
            try
            {
                coreService = CoreServiceFactory.GetCoreServiceContext(new Uri(ConfigurationSettings.AppSettings["CoreServiceURL"].ToString()), new NetworkCredential(ConfigurationSettings.AppSettings["UserName"].ToString(), ConfigurationSettings.AppSettings["Password"].ToString(), ConfigurationSettings.AppSettings["Domain"].ToString()));
                SchemaFieldsData schemaFieldData = coreService.Client.ReadSchemaFields(ConfigurationSettings.AppSettings["SchemaID"].ToString(), true, new ReadOptions());
                foreach (Article p in article)
                {
                    string serializeXml = "";
                    bool bln = Helper.helper.Serialize<Article>(p, ref serializeXml);
                    string xml = serializeXml;
                   var a= TridionComponent.GenerateComponent(coreService, xml, helper.SetPublication(ConfigurationSettings.AppSettings["FolderLocation"].ToString(), ConfigurationSettings.AppSettings["SchemaID"].ToString()), helper.SchemaType.Component, ConfigurationSettings.AppSettings["FolderLocation"].ToString(), p.title, p.title);
                   
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
           
        }
    }
}
