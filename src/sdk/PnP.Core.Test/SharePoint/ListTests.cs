﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using PnP.Core.Model.SharePoint;
using PnP.Core.Test.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PnP.Core.QueryModel;
using PnP.Core.Model;

namespace PnP.Core.Test.SharePoint
{
    [TestClass]
    public class ListTests
    {
        [ClassInitialize]
        public static void TestFixtureSetup(TestContext context)
        {
            // Configure mocking default for all tests in this class, unless override by a specific test
            //TestCommon.Instance.Mocking = false;
        }

        [TestMethod]
        public async Task GetListDataByRenderListDataAsStream()
        {
            //TestCommon.Instance.Mocking = false;
            using (var context = await TestCommon.Instance.GetContextAsync(TestCommon.TestSite))
            {
                // Create a new list
                string listTitle = "GetListDataByRenderListDataAsStream";
                var myList = context.Web.Lists.GetByTitle(listTitle);

                if (TestCommon.Instance.Mocking && myList != null)
                {
                    Assert.Inconclusive("Test data set should be setup to not have the list available.");
                }

                if (myList == null)
                {
                    myList = await context.Web.Lists.AddAsync(listTitle, ListTemplateType.GenericList);
                }

                // Add items to the list
                for (int i = 0; i < 10; i++)
                {
                    Dictionary<string, object> values = new Dictionary<string, object>
                        {
                            { "Title", $"Item {i}" }
                        };

                    await myList.Items.AddBatchAsync(values);
                }
                await context.ExecuteAsync();

                using (var context2 = await TestCommon.Instance.GetContextAsync(TestCommon.TestSite, 1))
                {
                    var list2 = context2.Web.Lists.GetByTitle(listTitle);
                    if (list2 != null)
                    {
                        var result = await list2.LoadListDataAsStreamAsync(new RenderListDataOptions() { ViewXml = "<View><ViewFields><FieldRef Name='Title' /></ViewFields><RowLimit>5</RowLimit></View>", RenderOptions = RenderListDataOptionsFlags.ListData });
                        Assert.IsTrue(list2.Items.Length == 5);
                        Assert.IsTrue(result.ContainsKey("FirstRow"));
                        Assert.IsTrue(result.ContainsKey("LastRow"));
                        Assert.IsTrue(result.ContainsKey("RowLimit"));
                        Assert.IsTrue((int)result["RowLimit"] == 5);
                    }
                }

                using (var context3 = await TestCommon.Instance.GetContextAsync(TestCommon.TestSite, 2))
                {
                    var list3 = context3.Web.Lists.GetByTitle(listTitle);
                    if (list3 != null)
                    {
                        var result = list3.LoadListDataAsStream(new RenderListDataOptions() { ViewXml = "<View><ViewFields><FieldRef Name='Title' /></ViewFields><RowLimit>5</RowLimit></View>", RenderOptions = RenderListDataOptionsFlags.ListData });
                        Assert.IsTrue(list3.Items.Length == 5);
                        Assert.IsTrue(result.ContainsKey("FirstRow"));
                        Assert.IsTrue(result.ContainsKey("LastRow"));
                        Assert.IsTrue(result.ContainsKey("RowLimit"));
                        Assert.IsTrue((int)result["RowLimit"] == 5);
                    }
                }

                // Cleanup the created list
                await myList.DeleteAsync();
            }
        }


        [TestMethod]
        public async Task ListLinqGetMethods()
        {
            //TestCommon.Instance.Mocking = false;
            using (var context = await TestCommon.Instance.GetContextAsync(TestCommon.TestSite))
            {
                // Create a new list
                string listTitle = "ListLinqGetMethods";
                var myList = context.Web.Lists.GetByTitle(listTitle);

                if (TestCommon.Instance.Mocking && myList != null)
                {
                    Assert.Inconclusive("Test data set should be setup to not have the list available.");
                }

                if (myList == null)
                {
                    myList = await context.Web.Lists.AddAsync(listTitle, ListTemplateType.GenericList);
                }

                var listGuid = myList.Id;

                using (var context2 = await TestCommon.Instance.GetContextAsync(TestCommon.TestSite, 1))
                {
                    var list2 = context2.Web.Lists.GetByTitle(listTitle);
                    if (list2 != null)
                    {
                        Assert.IsTrue(list2.Title == listTitle);
                        Assert.IsTrue(list2.Id == listGuid);
                    }
                }

                using (var context3 = await TestCommon.Instance.GetContextAsync(TestCommon.TestSite, 2))
                {
                    context3.GraphFirst = false;

                    var list3 = context3.Web.Lists.GetById(listGuid, p => p.TemplateType, p => p.Title);
                    if (list3 != null)
                    {
                        Assert.IsTrue(list3.Title == listTitle);
                        Assert.IsTrue(list3.Id == listGuid);
                    }
                }

                // BERT - filtering on Id does not work for SharePoint Lists with Graph - will be fixed with the new 2.1 VROOM backend
                using (var context4 = await TestCommon.Instance.GetContextAsync(TestCommon.TestSite, 3))
                {
                    context4.GraphFirst = false;

                    var list4 = context4.Web.Lists.GetByTitle(listTitle);
                    if (list4 != null)
                    {
                        var listCheck = context4.Web.Lists.GetById(list4.Id);

                        Assert.IsTrue(listCheck.Title == listTitle);
                        Assert.IsTrue(listCheck.Id == listGuid);
                    }
                }


                // Cleanup the created list
                await myList.DeleteAsync();
            }
        }

        [TestMethod]
        public async Task ListLinqGetAsyncMethods()
        {
            //TestCommon.Instance.Mocking = false;
            using (var context = await TestCommon.Instance.GetContextAsync(TestCommon.TestSite))
            {
                // Create a new list
                string listTitle = "ListLinqGetAsyncMethods";
                var myList = context.Web.Lists.GetByTitle(listTitle);

                if (TestCommon.Instance.Mocking && myList != null)
                {
                    Assert.Inconclusive("Test data set should be setup to not have the list available.");
                }

                if (myList == null)
                {
                    myList = await context.Web.Lists.AddAsync(listTitle, ListTemplateType.GenericList);
                }

                var listGuid = myList.Id;

                using (var context2 = await TestCommon.Instance.GetContextAsync(TestCommon.TestSite, 1))
                {
                    var list2 = context2.Web.Lists.GetByTitle(listTitle);
                    if (list2 != null)
                    {
                        Assert.IsTrue(list2.Title == listTitle);
                        Assert.IsTrue(list2.Id == listGuid);
                    }
                }

                using (var context3 = await TestCommon.Instance.GetContextAsync(TestCommon.TestSite, 2))
                {
                    context3.GraphFirst = false;

                    var list3 = await context3.Web.Lists.GetByIdAsync(listGuid, p => p.TemplateType, p => p.Title);
                    if (list3 != null)
                    {
                        Assert.IsTrue(list3.Title == listTitle);
                        Assert.IsTrue(list3.Id == listGuid);
                    }
                }

                // Cleanup the created list
                await myList.DeleteAsync();
            }
        }

        [TestMethod]
        public async Task ListLinqGetExceptionMethods()
        {
            //TestCommon.Instance.Mocking = false;
            using (var context = await TestCommon.Instance.GetContextAsync(TestCommon.TestSite))
            {
                // Create a new list
                string listTitle = "ListLinqGetExceptionMethods";
                var myList = context.Web.Lists.GetByTitle(listTitle);

                if (TestCommon.Instance.Mocking && myList != null)
                {
                    Assert.Inconclusive("Test data set should be setup to not have the list available.");
                }

                if (myList == null)
                {
                    myList = await context.Web.Lists.AddAsync(listTitle, ListTemplateType.GenericList);
                }

                var listGuid = myList.Id;

                using (var context2 = await TestCommon.Instance.GetContextAsync(TestCommon.TestSite, 1))
                {
                    var list2 = context2.Web.Lists.GetByTitle(listTitle);
                    if (list2 != null)
                    {
                        // Commented now that the GetBy methods are actual methods on the interface versus extention methods
                        //await Assert.ThrowsExceptionAsync<ArgumentNullException>(async () => {
                        //    IListCollection list = null;
                        //    await list.GetByIdAsync(listGuid);
                        //});

                        await Assert.ThrowsExceptionAsync<ArgumentNullException>(async () =>
                        {
                            await context2.Web.Lists.GetByIdAsync(Guid.Empty, p => p.TemplateType, p => p.Title);
                        });

                        // Commented now that the GetBy methods are actual methods on the interface versus extention methods
                        //Assert.ThrowsException<ArgumentNullException>(() => {
                        //    IListCollection list = null;
                        //    list.GetById(listGuid);
                        //});

                        Assert.ThrowsException<ArgumentNullException>(() =>
                        {
                            context2.Web.Lists.GetById(Guid.Empty, p => p.TemplateType, p => p.Title);
                        });

                        // Commented now that the GetBy methods are actual methods on the interface versus extention methods
                        //Assert.ThrowsException<ArgumentNullException>(() => {
                        //    IListCollection list = null;
                        //    list.GetByTitle(listTitle);
                        //});

                        Assert.ThrowsException<ArgumentNullException>(() =>
                        {
                            context2.Web.Lists.GetByTitle(null);
                        });

                        // Commented now that the GetBy methods are actual methods on the interface versus extention methods
                        //await Assert.ThrowsExceptionAsync<ArgumentNullException>(async () => {
                        //    IListCollection list = null;
                        //    await list.GetByTitleAsync(listTitle);
                        //});

                        // Commented now that the GetBy methods are actual methods on the interface versus extention methods
                        //await Assert.ThrowsExceptionAsync<ArgumentNullException>(async () => {
                        //    IListCollection list = null;
                        //    await list.GetByTitleAsync(null);
                        //});

                    }
                }

                // Cleanup the created list
                await myList.DeleteAsync();
            }
        }

        [TestMethod]
        public async Task GetItemsByCAMLQuery()
        {
            //TestCommon.Instance.Mocking = false;
            using (var context = await TestCommon.Instance.GetContextAsync(TestCommon.TestSite))
            {
                // Create a new list
                string listTitle = "GetItemsByCAMLQuery";
                var myList = context.Web.Lists.GetByTitle(listTitle);

                if (TestCommon.Instance.Mocking && myList != null)
                {
                    Assert.Inconclusive("Test data set should be setup to not have the list available.");
                }

                if (myList == null)
                {
                    myList = await context.Web.Lists.AddAsync(listTitle, ListTemplateType.GenericList);
                }

                // Add items to the list
                for (int i = 0; i < 10; i++)
                {
                    Dictionary<string, object> values = new Dictionary<string, object>
                        {
                            { "Title", $"Item {i}" }
                        };

                    await myList.Items.AddBatchAsync(values);
                }
                await context.ExecuteAsync();

                using (var context2 = await TestCommon.Instance.GetContextAsync(TestCommon.TestSite, 1))
                {
                    var list2 = context2.Web.Lists.GetByTitle(listTitle);
                    if (list2 != null)
                    {
                        await list2.LoadItemsByCamlQueryAsync("<View><ViewFields><FieldRef Name='Title' /></ViewFields><RowLimit>5</RowLimit></View>");
                        Assert.IsTrue(list2.Items.Length == 5);
                    }
                }

                using (var context3 = await TestCommon.Instance.GetContextAsync(TestCommon.TestSite, 2))
                {
                    var list3 = context3.Web.Lists.GetByTitle(listTitle);
                    if (list3 != null)
                    {
                        await list3.LoadItemsByCamlQueryAsync(new CamlQueryOptions()
                        {
                            ViewXml = "<View><ViewFields><FieldRef Name='Title' /></ViewFields></View>",
                            DatesInUtc = true
                        });
                        Assert.IsTrue(list3.Items.Length == 10);
                    }
                }

                // Batch testing
                using (var context4 = await TestCommon.Instance.GetContextAsync(TestCommon.TestSite, 3))
                {
                    var list4 = context4.Web.Lists.GetByTitle(listTitle);
                    if (list4 != null)
                    {
                        // Perform 2 queries, the first one limited to 5 items, the second one without limits. Total should be 10 items
                        await list4.LoadItemsByCamlQueryBatchAsync(new CamlQueryOptions()
                        {
                            ViewXml = "<View><ViewFields><FieldRef Name='Title' /></ViewFields><RowLimit>5</RowLimit></View>",
                        });
                        await list4.LoadItemsByCamlQueryBatchAsync(new CamlQueryOptions()
                        {
                            ViewXml = "<View><ViewFields><FieldRef Name='Title' /></ViewFields></View>",
                        });
                        await context4.ExecuteAsync();

                        Assert.IsTrue(list4.Items.Length == 10);
                    }
                }

                // Cleanup the created list
                await myList.DeleteAsync();
            }
        }

        [TestMethod]
        public async Task GetItemsByCAMLQuerySimpleNonAsyncTest()
        {
            //TestCommon.Instance.Mocking = false;
            using (var context = await TestCommon.Instance.GetContextAsync(TestCommon.TestSite))
            {
                // Create a new list
                string listTitle = "GetItemsByCAMLQuerySimpleAsyncTest";
                var myList = context.Web.Lists.GetByTitle(listTitle);

                if (TestCommon.Instance.Mocking && myList != null)
                {
                    Assert.Inconclusive("Test data set should be setup to not have the list available.");
                }

                if (myList == null)
                {
                    myList = context.Web.Lists.Add(listTitle, ListTemplateType.GenericList);
                }

                // Add items to the list
                for (int i = 0; i < 10; i++)
                {
                    Dictionary<string, object> values = new Dictionary<string, object>
                        {
                            { "Title", $"Item {i}" }
                        };

                    myList.Items.AddBatch(values);
                }
                await context.ExecuteAsync();

                using (var context2 = await TestCommon.Instance.GetContextAsync(TestCommon.TestSite, 1))
                {
                    var list2 = context2.Web.Lists.GetByTitle(listTitle);
                    if (list2 != null)
                    {
                        list2.LoadItemsByCamlQuery("<View><ViewFields><FieldRef Name='Title' /></ViewFields><RowLimit>5</RowLimit></View>");
                        Assert.IsTrue(list2.Items.Length == 5);
                    }
                }

                using (var context3 = await TestCommon.Instance.GetContextAsync(TestCommon.TestSite, 2))
                {
                    var list3 = context3.Web.Lists.GetByTitle(listTitle);
                    if (list3 != null)
                    {
                        list3.LoadItemsByCamlQuery(new CamlQueryOptions()
                        {
                            ViewXml = "<View><ViewFields><FieldRef Name='Title' /></ViewFields></View>",
                            DatesInUtc = true
                        });
                        Assert.IsTrue(list3.Items.Length == 10);
                    }
                }

                // Batch testing
                using (var context4 = await TestCommon.Instance.GetContextAsync(TestCommon.TestSite, 3))
                {
                    var list4 = context4.Web.Lists.GetByTitle(listTitle);
                    if (list4 != null)
                    {
                        // Perform 2 queries, the first one limited to 5 items, the second one without limits. Total should be 10 items
                        list4.LoadItemsByCamlQueryBatch(new CamlQueryOptions()
                        {
                            ViewXml = "<View><ViewFields><FieldRef Name='Title' /></ViewFields><RowLimit>5</RowLimit></View>",
                        });
                        list4.LoadItemsByCamlQueryBatch("<View><ViewFields><FieldRef Name='Title' /></ViewFields></View>");
                        await context4.ExecuteAsync();

                        Assert.IsTrue(list4.Items.Length == 10);
                    }
                }

                // Batch testing
                using (var context5 = await TestCommon.Instance.GetContextAsync(TestCommon.TestSite, 4))
                {
                    var list5 = context5.Web.Lists.GetByTitle(listTitle);
                    if (list5 != null)
                    {
                        var newBatch = context5.NewBatch();
                        // Perform 2 queries, the first one limited to 5 items, the second one without limits. Total should be 10 items
                        list5.LoadItemsByCamlQueryBatch(newBatch, new CamlQueryOptions()
                        {
                            ViewXml = "<View><ViewFields><FieldRef Name='Title' /></ViewFields><RowLimit>5</RowLimit></View>",
                        });
                        list5.LoadItemsByCamlQueryBatch(newBatch, new CamlQueryOptions()
                        {
                            ViewXml = "<View><ViewFields><FieldRef Name='Title' /></ViewFields></View>",
                        });
                        context5.ExecuteAsync(newBatch).GetAwaiter().GetResult();

                        Assert.IsTrue(list5.Items.Length == 10);
                    }
                }

                using (var context6 = await TestCommon.Instance.GetContextAsync(TestCommon.TestSite, 5))
                {
                    var list6 = context6.Web.Lists.GetByTitle(listTitle);
                    if (list6 != null)
                    {
                        var newBatch = context6.NewBatch();
                        // Perform 2 queries, the first one limited to 5 items, the second one without limits. Total should be 10 items
                        list6.LoadItemsByCamlQueryBatch(newBatch, "<View><ViewFields><FieldRef Name='Title' /></ViewFields><RowLimit>5</RowLimit></View>");
                        context6.ExecuteAsync(newBatch).GetAwaiter().GetResult();

                        Assert.IsTrue(list6.Items.Length == 5);
                    }
                }

                // Cleanup the created list
                await myList.DeleteAsync();
            }
        }

        [TestMethod]
        public async Task GetItemsByCAMLQueryOnCustomField()
        {
            //TestCommon.Instance.Mocking = false;
            using (var context = await TestCommon.Instance.GetContextAsync(TestCommon.TestSite))
            {
                // Create a new list
                string listTitle = TestCommon.GetPnPSdkTestAssetName("GetItemsByCAMLQueryOnCustomField");
                var myList = context.Web.Lists.GetByTitle(listTitle);

                if (TestCommon.Instance.Mocking && myList != null)
                {
                    Assert.Inconclusive("Test data set should be setup to not have the list available.");
                }

                IField customField;
                if (myList == null)
                {
                    myList = await context.Web.Lists.AddAsync(listTitle, ListTemplateType.GenericList);
                    customField = await myList.Fields.AddTextAsync("CustomField", new FieldTextOptions()
                    {
                        Group = "Custom fields",
                        AddToDefaultView = true,
                    });
                }

                // Add items to the list
                for (int i = 0; i < 10; i++)
                {
                    Dictionary<string, object> values = new Dictionary<string, object>
                        {
                            { "Title", $"Item {i}" },
                            { "CustomField", $"Field{i}" }
                        };

                    await myList.Items.AddBatchAsync(values);
                }
                await context.ExecuteAsync();

                using (var context2 = await TestCommon.Instance.GetContextAsync(TestCommon.TestSite, 1))
                {
                    var list2 = context2.Web.Lists.GetByTitle(listTitle);
                    if (list2 != null)
                    {
                        string query = @"<View>
                                          <ViewFields>
                                            <FieldRef Name='Title' />
                                            <FieldRef Name='CustomField' />
                                          </ViewFields>
                                          <Query>
                                            <Where>
                                              <Eq>
                                                <FieldRef Name='CustomField'/>
                                                <Value Type='text'>Field6</Value>
                                              </Eq>
                                            </Where>
                                          </Query>
                                        </View>";

                        await list2.LoadItemsByCamlQueryAsync(new CamlQueryOptions()
                        {
                            ViewXml = query,
                            DatesInUtc = true
                        });
                        Assert.IsTrue(list2.Items.Length == 1);
                        Assert.IsTrue(list2.Items.AsRequested().First()["CustomField"].ToString() == "Field6");
                    }
                }


                // Cleanup the created list
                await myList.DeleteAsync();
            }
        }

        [TestMethod]
        public async Task RecycleList()
        {
            //TestCommon.Instance.Mocking = false;
            using (var context = await TestCommon.Instance.GetContextAsync(TestCommon.TestSite))
            {
                // Create a new list
                await context.Web.LoadAsync(p => p.Lists);

                var web = context.Web;

                int listCount = web.Lists.Length;

                string listTitle = "RecycleList";
                var myList = web.Lists.AsRequested().FirstOrDefault(p => p.Title.Equals(listTitle, StringComparison.InvariantCultureIgnoreCase));

                if (TestCommon.Instance.Mocking && myList != null)
                {
                    Assert.Inconclusive("Test data set should be setup to not have the list available.");
                }

                if (myList == null)
                {
                    myList = await web.Lists.AddAsync(listTitle, ListTemplateType.GenericList);
                }

                // recycle the list
                var recycleBinItemId = await myList.RecycleAsync();
                // A valid recycle returns a recuycle bin item id
                Assert.IsTrue(recycleBinItemId != Guid.Empty);
                // The recycled list should have been deleted from the lists collection
                Assert.IsTrue(web.Lists.Length == listCount);
                // Loading lists again should still result in the same original list count as the added list is in the recycle bin
                await context.Web.LoadAsync(p => p.Lists);
                Assert.IsTrue(web.Lists.Length == listCount);

            }
        }

        [TestMethod]
        public async Task RecycleListBatch()
        {
            //TestCommon.Instance.Mocking = false;
            using (var context = await TestCommon.Instance.GetContextAsync(TestCommon.TestSite))
            {
                // Create a new list
                await context.Web.LoadAsync(p => p.Lists);

                var web = context.Web;
                
                int listCount = web.Lists.Length;

                string listTitle = TestCommon.GetPnPSdkTestAssetName("RecycleListBatch");
                var myList = web.Lists.AsRequested().FirstOrDefault(p => p.Title.Equals(listTitle, StringComparison.InvariantCultureIgnoreCase));

                if (TestCommon.Instance.Mocking && myList != null)
                {
                    Assert.Inconclusive("Test data set should be setup to not have the list available.");
                }

                if (myList == null)
                {
                    myList = await web.Lists.AddAsync(listTitle, ListTemplateType.GenericList);
                }

                // recycle the list
                var recycleBatchResult = await myList.RecycleBatchAsync();
                Assert.IsFalse(recycleBatchResult.IsAvailable);
                // Execute the batch
                await context.ExecuteAsync();
                Assert.IsTrue(recycleBatchResult.IsAvailable);
                Assert.AreNotEqual(Guid.Empty, recycleBatchResult.Result.Value);

                // The recycled list should have been deleted from the lists collection
                Assert.IsTrue(web.Lists.Length == listCount);
                // Loading lists again should still result in the same original list count as the added list is in the recycle bin
                await context.Web.GetAsync(p => p.Lists);
                Assert.IsTrue(web.Lists.Length == listCount);

            }
        }

        [TestMethod]
        public async Task GetListIRMSettingsTest()
        {
            //TestCommon.Instance.Mocking = false;

            using (var context = await TestCommon.Instance.GetContextAsync(TestCommon.TestSite))
            {
                // Create a new list
                string listTitle = "GetListIRMSettingsTest";
                var myList = context.Web.Lists.GetByTitle(listTitle);

                if (TestCommon.Instance.Mocking && myList != null)
                {
                    Assert.Inconclusive("Test data set should be setup to not have the list available.");
                }

                if (myList == null)
                {
                    myList = await context.Web.Lists.AddAsync(listTitle, ListTemplateType.DocumentLibrary);
                }

                // Enable IRM on the library
                await myList.LoadAsync();
                myList.IrmEnabled = true;
                await myList.UpdateAsync();

                // Load IRM settings
                await myList.InformationRightsManagementSettings.LoadAsync();

                // Verify default IRM list settings are returned
                Assert.IsTrue(myList.InformationRightsManagementSettings.Requested);
                Assert.IsTrue(myList.InformationRightsManagementSettings.AllowPrint == false);
                Assert.IsTrue(myList.InformationRightsManagementSettings.AllowScript == false);
                Assert.IsTrue(myList.InformationRightsManagementSettings.AllowWriteCopy == false);
                Assert.IsTrue(myList.InformationRightsManagementSettings.DisableDocumentBrowserView == false);
                Assert.IsTrue(myList.InformationRightsManagementSettings.DocumentAccessExpireDays == 90);
                Assert.IsTrue(myList.InformationRightsManagementSettings.DocumentLibraryProtectionExpireDate > new DateTime(2021, 1, 1));
                Assert.IsTrue(myList.InformationRightsManagementSettings.EnableDocumentAccessExpire == false);
                Assert.IsTrue(myList.InformationRightsManagementSettings.EnableDocumentBrowserPublishingView == false);
                Assert.IsTrue(myList.InformationRightsManagementSettings.EnableGroupProtection == false);
                Assert.IsTrue(myList.InformationRightsManagementSettings.EnableLicenseCacheExpire == false);
                Assert.IsTrue(myList.InformationRightsManagementSettings.GroupName == "");
                Assert.IsTrue(myList.InformationRightsManagementSettings.LicenseCacheExpireDays == 30);
                Assert.IsTrue(myList.InformationRightsManagementSettings.PolicyDescription == "");
                Assert.IsTrue(myList.InformationRightsManagementSettings.PolicyTitle == "");
                Assert.IsTrue(myList.InformationRightsManagementSettings.TemplateId == "");

                // turn off IRM again
                myList.IrmEnabled = false;
                await myList.UpdateAsync();

                // delete the list
                await myList.DeleteAsync();

            }
        }

        [TestMethod]
        public async Task GetListIRMSettingsBatchTest()
        {
            //TestCommon.Instance.Mocking = false;

            using (var context = await TestCommon.Instance.GetContextAsync(TestCommon.TestSite))
            {
                // Create a new list
                string listTitle = "GetListIRMSettingsBatchTest";
                var myList = context.Web.Lists.GetByTitle(listTitle);

                if (TestCommon.Instance.Mocking && myList != null)
                {
                    Assert.Inconclusive("Test data set should be setup to not have the list available.");
                }

                if (myList == null)
                {
                    myList = await context.Web.Lists.AddAsync(listTitle, ListTemplateType.DocumentLibrary);
                }

                // Enable IRM on the library
                await myList.LoadAsync();
                myList.IrmEnabled = true;
                await myList.UpdateAsync();

                // Load IRM settings
                await myList.InformationRightsManagementSettings.LoadBatchAsync();
                await context.ExecuteAsync();

                // Verify default IRM list settings are returned
                Assert.IsTrue(myList.InformationRightsManagementSettings.Requested);
                Assert.IsTrue(myList.InformationRightsManagementSettings.AllowPrint == false);
                Assert.IsTrue(myList.InformationRightsManagementSettings.AllowScript == false);
                Assert.IsTrue(myList.InformationRightsManagementSettings.AllowWriteCopy == false);
                Assert.IsTrue(myList.InformationRightsManagementSettings.DisableDocumentBrowserView == false);
                Assert.IsTrue(myList.InformationRightsManagementSettings.DocumentAccessExpireDays == 90);
                Assert.IsTrue(myList.InformationRightsManagementSettings.DocumentLibraryProtectionExpireDate > new DateTime(2021, 1, 1));
                Assert.IsTrue(myList.InformationRightsManagementSettings.EnableDocumentAccessExpire == false);
                Assert.IsTrue(myList.InformationRightsManagementSettings.EnableDocumentBrowserPublishingView == false);
                Assert.IsTrue(myList.InformationRightsManagementSettings.EnableGroupProtection == false);
                Assert.IsTrue(myList.InformationRightsManagementSettings.EnableLicenseCacheExpire == false);
                Assert.IsTrue(myList.InformationRightsManagementSettings.GroupName == "");
                Assert.IsTrue(myList.InformationRightsManagementSettings.LicenseCacheExpireDays == 30);
                Assert.IsTrue(myList.InformationRightsManagementSettings.PolicyDescription == "");
                Assert.IsTrue(myList.InformationRightsManagementSettings.PolicyTitle == "");
                Assert.IsTrue(myList.InformationRightsManagementSettings.TemplateId == "");

                // turn off IRM again
                myList.IrmEnabled = false;
                await myList.UpdateAsync();

                // delete the list
                await myList.DeleteAsync();

            }
        }

        [TestMethod]
        public async Task GetListByServerRelativeUrl()
        {
            //TestCommon.Instance.Mocking = false;
            using (var context = await TestCommon.Instance.GetContextAsync(TestCommon.TestSite))
            {
                var list = await context.Web.Lists.GetByServerRelativeUrlAsync($"{context.Uri.LocalPath}/shared%20documents", p => p.Title, p => p.ListExperience);

                Assert.IsTrue(list.Requested);
                Assert.AreEqual(list.Title, "documents", true);

            }
        }

        [TestMethod]
        public async Task GetListByServerRelativeUrlSync()
        {
            //TestCommon.Instance.Mocking = false;
            using (var context = await TestCommon.Instance.GetContextAsync(TestCommon.TestSite))
            {
                var list = context.Web.Lists.GetByServerRelativeUrl($"{context.Uri.LocalPath}/shared%20documents", p => p.Title, p => p.ListExperience);

                Assert.IsTrue(list.Requested);
                Assert.AreEqual(list.Title, "documents", true);

            }
        }

        [TestMethod]
        public async Task GetListByTitle()
        {
            //TestCommon.Instance.Mocking = false;
            using (var context = await TestCommon.Instance.GetContextAsync(TestCommon.TestSite))
            {
                var list = await context.Web.Lists.GetByTitleAsync("Documents", p => p.Title, p => p.ListExperience);

                Assert.IsTrue(list.Requested);
                Assert.AreEqual(list.Title, "documents", true);

            }
        }

        [TestMethod]
        public async Task GetListByTitleWithQueryPropertiesSync()
        {
            //TestCommon.Instance.Mocking = false;
            using (var context = await TestCommon.Instance.GetContextAsync(TestCommon.TestSite))
            {
                var list = context.Web.Lists.GetByTitle("Documents", p => p.Title, p => p.Items, p => p.Fields.QueryProperties(p => p.InternalName, p => p.FieldTypeKind, p => p.TypeAsString, p => p.Title));

                Assert.IsTrue(list.Requested);
                Assert.AreEqual(list.Title, "documents", true);
            }
        }

        [TestMethod]
        public async Task GetListByTitleSync()
        {
            //TestCommon.Instance.Mocking = false;
            using (var context = await TestCommon.Instance.GetContextAsync(TestCommon.TestSite))
            {
                var list = context.Web.Lists.GetByTitle("Documents", p => p.Title, p => p.ListExperience);

                Assert.IsTrue(list.Requested);
                Assert.AreEqual(list.Title, "documents", true);
            }
        }

        [TestMethod]
        public async Task GetListByTitleWithExpand()
        {
            //TestCommon.Instance.Mocking = false;
            using (var context = await TestCommon.Instance.GetContextAsync(TestCommon.TestSite))
            {
                var list = context.Web.Lists.GetByTitle("Documents", p => p.Title, p => p.ContentTypes);

                Assert.IsTrue(list.Requested);
                Assert.AreEqual(list.Title, "documents", true);
                Assert.IsTrue(list.ContentTypes.Requested);
                Assert.IsTrue(list.ContentTypes.AsRequested().First().IsPropertyAvailable(p => p.Id));
                Assert.IsTrue(list.ContentTypes.AsRequested().First().IsPropertyAvailable(p => p.Description));
            }
        }

        [TestMethod]
        public async Task GetListByTitleWithLoad()
        {
            //TestCommon.Instance.Mocking = false;
            using (var context = await TestCommon.Instance.GetContextAsync(TestCommon.TestSite))
            {
                var list = await context.Web.Lists.GetByTitleAsync("Documents", p => p.Title, p => p.ListExperience, p => p.ContentTypes.QueryProperties(p => p.Id, p => p.Name));

                Assert.IsTrue(list.Requested);
                Assert.AreEqual(list.Title, "documents", true);
                Assert.IsTrue(list.ContentTypes.Requested);
                Assert.IsTrue(list.ContentTypes.AsRequested().First().IsPropertyAvailable(p => p.Id));
                Assert.IsTrue(list.ContentTypes.AsRequested().First().IsPropertyAvailable(p => p.Name));
            }
        }

        [TestMethod]
        public async Task GetListByTitleWithLoadRecursive()
        {
            //TestCommon.Instance.Mocking = false;
            using (var context = await TestCommon.Instance.GetContextAsync(TestCommon.TestSite))
            {
                var list = await context.Web.Lists.GetByTitleAsync("Documents", p => p.Title, p => p.ListExperience,
                    p => p.ContentTypes.QueryProperties(p => p.Id, p => p.Name,
                        p => p.FieldLinks.QueryProperties(p => p.Id, p => p.Name)));

                Assert.IsTrue(list.Requested);
                Assert.AreEqual(list.Title, "documents", true);
                Assert.IsTrue(list.ContentTypes.Requested);

                var firstContentType = list.ContentTypes.AsRequested().First();
                Assert.IsTrue(firstContentType.IsPropertyAvailable(p => p.Id));
                Assert.IsTrue(firstContentType.IsPropertyAvailable(p => p.Name));
                Assert.IsTrue(firstContentType.FieldLinks.Requested);

                var firstFieldLink = firstContentType.FieldLinks.AsRequested().First();
                Assert.IsTrue(firstFieldLink.IsPropertyAvailable(p => p.Id));
                Assert.IsTrue(firstFieldLink.IsPropertyAvailable(p => p.Name));
                Assert.IsTrue(!string.IsNullOrEmpty(firstFieldLink.Name));
            }
        }

        [TestMethod]
        public async Task GetListByIdFollowedByAdd()
        {
            //TestCommon.Instance.Mocking = false;
            using (var context = await TestCommon.Instance.GetContextAsync(TestCommon.TestSite))
            {
                // Create a new list
                string listTitle = TestCommon.GetPnPSdkTestAssetName("GetListByIdWithAdd");
                var myList = context.Web.Lists.GetByTitle(listTitle);

                if (TestCommon.Instance.Mocking && myList != null)
                {
                    Assert.Inconclusive("Test data set should be setup to not have the list available.");
                }

                if (myList == null)
                {
                    myList = await context.Web.Lists.AddAsync(listTitle, ListTemplateType.GenericList);
                }

                Guid listId = myList.Id;

                using (var context2 = await TestCommon.Instance.GetContextAsync(TestCommon.TestSite, 2))
                {
                    // Filtering lists by Id is not yet supported by Microsoft Graph
                    context2.GraphFirst = false;

                    // Get list without root folder - will trigger rootfolder load
                    var list = await context2.Web.Lists.GetByIdAsync(listId,
                        l => l.Fields.QueryProperties(f => f.Id, f => f.Title, f => f.InternalName, f => f.TypeAsString));

                    // Add a list item
                    Dictionary<string, object> values = new Dictionary<string, object>
                    {
                        { "Title", "Yes" }
                    };

                    await list.Items.AddAsync(values);

                    // Get list with roorfolder, more optimized
                    list = await context2.Web.Lists.GetByIdAsync(listId,
                        l => l.RootFolder, l => l.Fields.QueryProperties(f => f.Id, f => f.Title, f => f.InternalName, f => f.TypeAsString));

                    await list.Items.AddAsync(values);

                    using (var context3 = await TestCommon.Instance.GetContextAsync(TestCommon.TestSite, 3))
                    {
                        //context3.GraphFirst = false;

                        // We should have 2 list items
                        var list3 = await context3.Web.Lists.GetByIdAsync(listId, p => p.Items);

                        Assert.IsTrue(list3.Items.Length == 2);
                    }

                    // delete the list again
                    await list.DeleteAsync();
                }
            }
        }

        [TestMethod]
        public async Task BreakRoleInheritanceTest()
        {
            //TestCommon.Instance.Mocking = false;

            using (var context = await TestCommon.Instance.GetContextAsync(TestCommon.TestSite))
            {
                context.Web.Load(p => p.Lists);

                var web = context.Web;

                string listTitle = TestCommon.GetPnPSdkTestAssetName("BreakRoleInheritanceTest");
                var myList = web.Lists.AsRequested().FirstOrDefault(p => p.Title.Equals(listTitle, StringComparison.InvariantCultureIgnoreCase));

                if (myList != null)
                {
                    Assert.Inconclusive("Test data set should be setup to not have the list available.");
                }

              
                myList = web.Lists.Add(listTitle, ListTemplateType.GenericList);

                await myList.BreakRoleInheritanceAsync(false, false);

                await myList.EnsurePropertiesAsync(l => l.HasUniqueRoleAssignments);

                Assert.IsTrue(myList.HasUniqueRoleAssignments);
                await myList.DeleteAsync();
            }
        }

        [TestMethod]
        public async Task ResetRoleInheritanceTest()
        {
            //TestCommon.Instance.Mocking = false;

            using (var context = await TestCommon.Instance.GetContextAsync(TestCommon.TestSite))
            {
                context.Web.Load(p => p.Lists);

                var web = context.Web;

                string listTitle = TestCommon.GetPnPSdkTestAssetName("ResetRoleInheritanceTest");
                var myList = web.Lists.AsRequested().FirstOrDefault(p => p.Title.Equals(listTitle, StringComparison.InvariantCultureIgnoreCase));

                if (myList != null)
                {
                    Assert.Inconclusive("Test data set should be setup to not have the list available.");
                }

                myList = web.Lists.Add(listTitle, ListTemplateType.GenericList);

                await myList.BreakRoleInheritanceAsync(false, false);

                await myList.ResetRoleInheritanceAsync();

                await myList.EnsurePropertiesAsync(l => l.HasUniqueRoleAssignments);

                Assert.IsFalse(myList.HasUniqueRoleAssignments);

                await myList.DeleteAsync();
            }
        }

        [TestMethod]
        public async Task GetRoleDefinitionsTest()
        {
            //TestCommon.Instance.Mocking = false;

            using (var context = await TestCommon.Instance.GetContextAsync(TestCommon.TestSite))
            {
                context.Web.Load(p => p.Lists, p => p.CurrentUser);

                var web = context.Web;

                string listTitle = TestCommon.GetPnPSdkTestAssetName("GetRoleDefinitionsTest");
                var myList = web.Lists.AsRequested().FirstOrDefault(p => p.Title.Equals(listTitle, StringComparison.InvariantCultureIgnoreCase));

                if (myList != null)
                {
                    Assert.Inconclusive("Test data set should be setup to not have the list available.");
                }

                myList = web.Lists.Add(listTitle, ListTemplateType.GenericList);

                await myList.BreakRoleInheritanceAsync(false, false);

                var roleDefinitions = await myList.GetRoleDefinitionsAsync(web.CurrentUser.Id);

                Assert.IsTrue(roleDefinitions.Length > 0);

                await myList.DeleteAsync();
            }
        }

        [TestMethod]
        public async Task AddRoleDefinitionsTest()
        {
            //TestCommon.Instance.Mocking = false;

            using (var context = await TestCommon.Instance.GetContextAsync(TestCommon.TestSite))
            {
                string listTitle = TestCommon.GetPnPSdkTestAssetName("AddRoleDefinitionsTest");
                string roleDefName = TestCommon.GetPnPSdkTestAssetName("AddRoleDefinitionsTest");

                context.Web.Load(p => p.Lists, p => p.CurrentUser);

                var web = context.Web;

                var roleDefinition = web.RoleDefinitions.Add(roleDefName, RoleType.Administrator, new PermissionKind[] { PermissionKind.AddAndCustomizePages });
                                
                var myList = web.Lists.AsRequested().FirstOrDefault(p => p.Title.Equals(listTitle, StringComparison.InvariantCultureIgnoreCase));

                if (myList != null)
                {
                    Assert.Inconclusive("Test data set should be setup to not have the list available.");
                }

                myList = web.Lists.Add(listTitle, ListTemplateType.GenericList);

                await myList.BreakRoleInheritanceAsync(false, false);

                myList.AddRoleDefinitions(web.CurrentUser.Id, roleDefName);

                var roleDefinitions = await myList.GetRoleDefinitionsAsync(web.CurrentUser.Id);

                Assert.IsTrue(roleDefinitions.Length > 1 && roleDefinitions.AsRequested().FirstOrDefault(r => r.Name == roleDefName) != null);

                await myList.DeleteAsync();

                await roleDefinition.DeleteAsync();
            }
        }

        [TestMethod]
        public async Task AddRoleDefinitionsSpecialCharsTest()
        {
            //TestCommon.Instance.Mocking = false;

            using (var context = await TestCommon.Instance.GetContextAsync(TestCommon.TestSite))
            {
                string listTitle = TestCommon.GetPnPSdkTestAssetName("AddRoleDefinitionsSpecialCharsTest");
                string roleDefName = "Fullständig Behörighet";

                context.Web.Load(p => p.Lists, p => p.CurrentUser);

                var web = context.Web;

                var roleDefinition = web.RoleDefinitions.Add(roleDefName, RoleType.Administrator, new PermissionKind[] { PermissionKind.AddAndCustomizePages });

                var myList = web.Lists.AsRequested().FirstOrDefault(p => p.Title.Equals(listTitle, StringComparison.InvariantCultureIgnoreCase));

                if (myList != null)
                {
                    Assert.Inconclusive("Test data set should be setup to not have the list available.");
                }

                myList = web.Lists.Add(listTitle, ListTemplateType.GenericList);

                await myList.BreakRoleInheritanceAsync(false, false);

                myList.AddRoleDefinitions(web.CurrentUser.Id, roleDefName);

                var roleDefinitions = await myList.GetRoleDefinitionsAsync(web.CurrentUser.Id);

                Assert.IsTrue(roleDefinitions.Length > 1 && roleDefinitions.AsRequested().FirstOrDefault(r => r.Name == roleDefName) != null);

                await myList.DeleteAsync();

                await roleDefinition.DeleteAsync();
            }
        }

        [TestMethod]
        public async Task RemoveRoleDefinitionsTest()
        {
            //TestCommon.Instance.Mocking = false;

            using (var context = await TestCommon.Instance.GetContextAsync(TestCommon.TestSite))
            {
                string listTitle = TestCommon.GetPnPSdkTestAssetName("RemoveRoleDefinitionsTest");
                string roleDefName = TestCommon.GetPnPSdkTestAssetName("RemoveRoleDefinitionsTest");

                context.Web.Load(p => p.Lists, p => p.CurrentUser);

                var web = context.Web;

                var roleDefinition = web.RoleDefinitions.Add(roleDefName, RoleType.Administrator, new PermissionKind[] { PermissionKind.AddAndCustomizePages });

                var myList = web.Lists.AsRequested().FirstOrDefault(p => p.Title.Equals(listTitle, StringComparison.InvariantCultureIgnoreCase));

                if (myList != null)
                {
                    Assert.Inconclusive("Test data set should be setup to not have the list available.");
                }

                myList = web.Lists.Add(listTitle, ListTemplateType.GenericList);

                await myList.BreakRoleInheritanceAsync(false, false);

                myList.AddRoleDefinitions(web.CurrentUser.Id, roleDefName);

                var roleDefinitionsBefore = await myList.GetRoleDefinitionsAsync(web.CurrentUser.Id);

                Assert.IsTrue(roleDefinitionsBefore.Length > 0 && roleDefinitionsBefore.AsRequested().FirstOrDefault(r => r.Name == roleDefName) != null);

                myList.RemoveRoleDefinitions(web.CurrentUser.Id, roleDefName);

                var roleDefinitionsAfter = await myList.GetRoleDefinitionsAsync(web.CurrentUser.Id);

                Assert.IsTrue(roleDefinitionsAfter.Length != roleDefinitionsBefore.Length && roleDefinitionsAfter.AsRequested().FirstOrDefault(r => r.Name == roleDefName) == null);

                await myList.DeleteAsync();

                await roleDefinition.DeleteAsync();
            }
        }


    }
}
