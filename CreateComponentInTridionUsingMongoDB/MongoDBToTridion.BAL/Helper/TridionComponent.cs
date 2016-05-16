using MongoDBToTridion.BAL.CoreServiceFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Tridion.ContentManager.CoreService.Client;

namespace MongoDBToTridion.BAL.Helper
{
    public class TridionComponent
    {
        #region Generate Component
        public static string GenerateComponent(ICoreServiceFrameworkContext coreService, string xml, string schemaID, Helper.helper.SchemaType schemaType, string folderUri, string ext_Id, string ext_Name)
        {
            try
            {

                string Title = string.Empty;
                string Tcmuri = string.Empty;
                string ReturnTcmuri = string.Empty;
                SearchQueryData filter = new SearchQueryData();
                filter.FullTextQuery = "title";
                filter.ItemTypes = new ItemType[] { ItemType.Component };

                BasedOnSchemaData basedSchema = new BasedOnSchemaData();
                basedSchema.Schema = new LinkToSchemaData() { IdRef = schemaID };
                basedSchema.Field = "title";
                basedSchema.FieldValue = ext_Id;


                filter.BasedOnSchemas = new BasedOnSchemaData[] { basedSchema };
                XElement results = coreService.Client.GetSearchResultsXml(filter);
                for (IEnumerator<XElement> e = results.Descendants().GetEnumerator(); e.MoveNext();)
                {
                    Title = e.Current.Attribute(XName.Get("title")).Value;
                    Tcmuri = e.Current.FirstAttribute.Value != null ? e.Current.FirstAttribute.Value : null;
                }
                Title = Title != string.Empty ? Title : ext_Name;
                ComponentData componentData = GetNewComponent(folderUri, schemaID, schemaType, Title);

                componentData.ComponentType = ComponentType.Normal;
                SchemaData sd = coreService.Client.Read(schemaID, null) as SchemaData;

                var content = XElement.Parse(xml);
                var xmlns = UpdateNodesWithDefaultNamespace(content.ToString(), "xmlns=" + "\"" + @"" + sd.NamespaceUri.ToString() + @"""");

                componentData.Content = xmlns.ToString().Replace("Article","Content");
                TridionObjectInfo tridionObjectInfo = Helper.helper.GetTridionObject(coreService, ItemType.Component, folderUri, Title);
                if (tridionObjectInfo.TcmUri != null)
                {
                    componentData.Id = tridionObjectInfo.TcmUri;
                    var data = (ComponentData)coreService.Client.Read(tridionObjectInfo.TcmUri, new ReadOptions());
                    componentData = (ComponentData)coreService.Client.Update(componentData, new ReadOptions());
                   
                    ReturnTcmuri = componentData.Id.ToString();
                }
                else
                {
                    componentData = (ComponentData)coreService.Client.Create(componentData, new ReadOptions());
                   
                    ReturnTcmuri = componentData.Id.ToString();
                }
                return ReturnTcmuri;
            }
            catch (Exception ex)
            {
                
                return "";
            }
        }
        #endregion
        private static String UpdateNodesWithDefaultNamespace(String xml, String defaultNamespace)
        {
            try
            {
                if (!String.IsNullOrEmpty(xml) && !String.IsNullOrEmpty(defaultNamespace))
                {
                    int currentIndex = 0;


                    //find index of tag opening character
                    int tagOpenIndex = "<Article".Length;

                    //no more tag openings are found
                    if (tagOpenIndex == -1)
                    {
                        return "";
                    }

                    //if it's a closing tag
                    if (xml[tagOpenIndex + 1] == '/')
                    {
                        currentIndex = tagOpenIndex + 1;
                    }
                    else
                    {
                        currentIndex = tagOpenIndex;
                    }

                    //find corresponding tag closing character
                    int tagCloseIndex = xml.IndexOf('>', tagOpenIndex);
                    if (tagCloseIndex <= tagOpenIndex)
                    {
                        throw new Exception("Invalid XML file.");
                    }

                    //look for a colon within currently processed tag
                    String currentTagSubstring = xml.Substring(tagOpenIndex, tagCloseIndex - tagOpenIndex);
                    int firstSpaceIndex = currentTagSubstring.IndexOf(' ');
                    int nameSpaceColonIndex;
                    //if space was found
                    if (firstSpaceIndex != -1)
                    {
                        //look for namespace colon between tag open character and the first space character
                        nameSpaceColonIndex = currentTagSubstring.IndexOf(':', 0, firstSpaceIndex);
                    }
                    else
                    {
                        //look for namespace colon between tag open character and tag close character
                        nameSpaceColonIndex = currentTagSubstring.IndexOf(':');
                    }

                    //if there is no namespace
                    if (nameSpaceColonIndex == -1)
                    {
                        //insert namespace after tag opening characters '<' or '</'
                        xml = xml.Insert(currentIndex + 1, String.Format("{0} ", defaultNamespace));
                    }

                    //look for next tags after current tag closing character
                    currentIndex = tagCloseIndex;

                }

                return xml;
            }
            catch (Exception ex)
            {
                 
                return "";
            }

        }
        #region Get New Blank Component Property
        public static ComponentData GetNewComponent(string folderUri, string schemaUri, Helper.helper.SchemaType schemaType, string title = null)
        {

            return new ComponentData
            {
                LocationInfo = new LocationInfo
                {
                    OrganizationalItem = new LinkToOrganizationalItemData
                    {
                        IdRef = folderUri
                        // WebDavUrl  = folderUri

                    },
                },

                ComponentType = schemaType == Helper.helper.SchemaType.Multimedia ? ComponentType.Multimedia : ComponentType.Normal,
                Title = title,

                Schema = new LinkToSchemaData
                {
                    IdRef = schemaUri //schemaData.IdRef
                    //WebDavUrl = schemaUri
                },

                IsBasedOnMandatorySchema = false,
                IsBasedOnTridionWebSchema = true,
                ApprovalStatus = new LinkToApprovalStatusData
                {
                    IdRef = "tcm:0-0-0"
                },
                Id = "tcm:0-0-0"
            };
        }
        #endregion
    }
}
