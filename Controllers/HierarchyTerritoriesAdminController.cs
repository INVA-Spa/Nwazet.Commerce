﻿using Nwazet.Commerce.Extensions;
using Nwazet.Commerce.Models;
using Nwazet.Commerce.Permissions;
using Nwazet.Commerce.Services;
using Nwazet.Commerce.ViewModels;
using Orchard;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Aspects;
using Orchard.ContentManagement.MetaData;
using Orchard.ContentManagement.MetaData.Models;
using Orchard.Core.Contents.Settings;
using Orchard.Data;
using Orchard.Environment.Extensions;
using Orchard.Localization;
using Orchard.Mvc.Extensions;
using Orchard.Security;
using Orchard.Security.Permissions;
using Orchard.UI.Admin;
using Orchard.UI.Notify;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.Routing;

namespace Nwazet.Commerce.Controllers {
    [OrchardFeature("Territories")]
    [Admin]
    [ValidateInput(false)]
    public class HierarchyTerritoriesAdminController : Controller, IUpdateModel {

        private readonly IContentManager _contentManager;
        private readonly IContentDefinitionManager _contentDefinitionManager;
        private readonly ITerritoriesService _territoriesService;
        private readonly IAuthorizer _authorizer;
        private readonly IWorkContextAccessor _workContextAccessor;
        private readonly RouteCollection _routeCollection;
        private readonly ITerritoriesHierarchyService _territoriesHierarchyService;
        private readonly ITerritoriesRepositoryService _territoriesRepositoryService;
        private readonly ITransactionManager _transactionManager;
        private readonly INotifier _notifier;

        public HierarchyTerritoriesAdminController(
            IContentManager contentManager,
            IContentDefinitionManager contentDefinitionManager,
            ITerritoriesService territoriesService,
            IAuthorizer authorizer,
            IWorkContextAccessor workContextAccessor,
            RouteCollection routeCollection,
            ITerritoriesHierarchyService territoriesHierarchyService,
            ITerritoriesRepositoryService territoriesRepositoryService,
            ITransactionManager transactionManager,
            INotifier notifier) {

            _contentManager = contentManager;
            _contentDefinitionManager = contentDefinitionManager;
            _territoriesService = territoriesService;
            _authorizer = authorizer;
            _workContextAccessor = workContextAccessor;
            _routeCollection = routeCollection;
            _territoriesHierarchyService = territoriesHierarchyService;
            _territoriesRepositoryService = territoriesRepositoryService;
            _transactionManager = transactionManager;
            _notifier = notifier;

            T = NullLocalizer.Instance;

            _allowedTerritoryTypes = new Lazy<IEnumerable<ContentTypeDefinition>>(GetAllowedTerritoryTypes);
            _allowedHierarchyTypes = new Lazy<IEnumerable<ContentTypeDefinition>>(GetAllowedHierarchyTypes);
        }

        public Localizer T;

        [HttpGet]
        public ActionResult Index(int id) {
            ActionResult redirectTo;
            if (ShouldRedirectForPermissions(id, out redirectTo)) {
                return redirectTo;
            }

            // list the first level of territories for the selected hierarchy
            // The null checks for these objects are done in ShouldRedirectForPermissions
            var hierarchyItem = _contentManager.Get(id, VersionOptions.Latest);
            var hierarchyPart = hierarchyItem.As<TerritoryHierarchyPart>();
            
            var firstLevelOfHierarchy = _territoriesService
                .GetTerritoriesQuery(hierarchyPart, null, VersionOptions.Latest)
                .List().ToList();
                       

            var model = new TerritoryHierarchyTerritoriesViewModel {
                HierarchyPart = hierarchyPart,
                HierarchyItem = hierarchyItem,
                FirstLevelNodes = firstLevelOfHierarchy.Select(MakeANode).ToList(),
                Nodes = _territoriesService.
                    GetTerritoriesQuery(hierarchyPart, VersionOptions.Latest)
                    .List().Select(MakeANode).ToList(),
                CanAddMoreTerritories = _territoriesService
                    .GetAvailableTerritoryInternals(hierarchyPart)
                    .Any()
            };

            return View(model);
        }

        #region Create
        [HttpGet]
        public ActionResult CreateTerritory(string id, int hierarchyId) {
            // id is the name of the ContentType for the territory we are trying to create. By calling
            // that argument "id" we can use the standard MVC routing (i.e. controller/action/id?querystring).
            // This is especially nice on POST calls.
            ActionResult redirectTo;
            if (ShouldRedirectForPermissions(hierarchyId, out redirectTo)) {
                return redirectTo;
            }

            // The null checks for these objects are done in ShouldRedirectForPermissions
            var hierarchyItem = _contentManager.Get(hierarchyId, VersionOptions.Latest);
            var hierarchyPart = hierarchyItem.As<TerritoryHierarchyPart>();
            var hierarchyTitle = _contentManager.GetItemMetadata(hierarchyItem).DisplayText;

            if (!id.Equals(hierarchyPart.TerritoryType, StringComparison.OrdinalIgnoreCase)) {
                // The hierarchy expects a TerritoryType different form the one we are trying to create
                var errorText = string.IsNullOrWhiteSpace(hierarchyTitle) ?
                    T("The requested type \"{0}\" does not match the expected TerritoryType for the hierarchy.", id) :
                    T("The requested type \"{0}\" does not match the expected TerritoryType for hierarchy \"{1}\".", id, hierarchyTitle);
                AddModelError("", errorText);
                return RedirectToAction("Index");
            }

            // There must be "unused" TerritoryInternalRecords for this hierarchy.
            if (_territoriesService
                .GetAvailableTerritoryInternals(hierarchyPart)
                .Any()) {

                // Creation
                var territoryItem = _contentManager.New(id);
                // Cannot insert Territory in the Hierarchy here, because its records do not exist yet.
                // We will have to do it in the POST call.
                // Allow user to Edit stuff
                var model = _contentManager.BuildEditor(territoryItem);
                return View(model.Hierarchy(hierarchyItem));
            }

            AddModelError("", T("There are no territories that may be added to hierarchy \"{1}\".", hierarchyTitle));
            return RedirectToAction("Index");
        }

        [HttpPost, ActionName("CreateTerritory")]
        [Orchard.Mvc.FormValueRequired("submit.Save")]
        public ActionResult CreateTerritoryPost(string id, int hierarchyId, string returnUrl) {
            return CreateTerritoryPost(id, hierarchyId, returnUrl, contentItem => {
                if (!contentItem.Has<IPublishingControlAspect>() &&
                    !contentItem.TypeDefinition.Settings.GetModel<ContentTypeSettings>().Draftable) {

                    _contentManager.Publish(contentItem);
                }
            });
        }

        [HttpPost, ActionName("CreateTerritory")]
        [Orchard.Mvc.FormValueRequired("submit.Publish")]
        public ActionResult CreateAndPublishTerritoryPost(string id, int hierarchyId, string returnUrl) {
            var dummyContent = _contentManager.New(id);

            if (!_authorizer.Authorize(
                Orchard.Core.Contents.Permissions.PublishContent, dummyContent, TerritoriesUtilities.Creation401TerritoryMessage))
                return new HttpUnauthorizedResult();

            return CreateTerritoryPost(id, hierarchyId, returnUrl, contentItem => _contentManager.Publish(contentItem));
        }

        private ActionResult CreateTerritoryPost(
            string typeName, int hierarchyId, string returnUrl, Action<ContentItem> conditionallyPublish) {

            return ExecuteTerritoryPost(new TerritoryExecutionContext {
                HierarchyItem = _contentManager.Get(hierarchyId),
                TerritoryItem = _contentManager.New(typeName),
                Message = TerritoriesUtilities.Creation401TerritoryMessage,
                AdditionalPermissions = new Permission[] { Orchard.Core.Contents.Permissions.EditContent },
                ExecutionAction = item => {
                    _contentManager.Create(item, VersionOptions.Draft);

                    var model = _contentManager.UpdateEditor(item, this);

                    if (!ModelState.IsValid) {
                        _transactionManager.Cancel();
                        return View(model);
                    }

                    var territoryPart = item.As<TerritoryPart>();

                    conditionallyPublish(item);

                    _notifier.Information(string.IsNullOrWhiteSpace(item.TypeDefinition.DisplayName)
                        ? T("Your content has been created.")
                        : T("Your {0} has been created.", item.TypeDefinition.DisplayName));

                    return this.RedirectLocal(returnUrl, () => 
                        RedirectToAction("EditTerritory", 
                            new RouteValueDictionary { { "Id", item.Id } }));
                }
            });
        }

        #endregion

        #region Edit
        [HttpGet]
        public ActionResult EditTerritory(int id) {

            var territoryItem = _contentManager.Get(id, VersionOptions.Latest);
            if (territoryItem == null)
                return HttpNotFound();
            var territoryPart = territoryItem.As<TerritoryPart>();
            if (territoryPart == null)
                return HttpNotFound();

            ActionResult redirectTo;
            if (ShouldRedirectForPermissions(territoryPart.Record.Hierarchy.Id, out redirectTo)) {
                return redirectTo;
            }
            
            if (!_authorizer.Authorize(
                Orchard.Core.Contents.Permissions.EditContent, territoryItem, TerritoriesUtilities.Edit401TerritoryMessage))
                return new HttpUnauthorizedResult();

            // We should have filtered out the cases where we cannot or should not be editing the item here
            var model = _contentManager.BuildEditor(territoryItem);
            return View(model);
        }
        #endregion

        private ActionResult ExecuteTerritoryPost(
            TerritoryExecutionContext context) {
            if (context.HierarchyItem == null || context.TerritoryItem == null) {
                return HttpNotFound();
            }
            var hierarchyPart = context.HierarchyItem.As<TerritoryHierarchyPart>();
            var territoryPart = context.TerritoryItem.As<TerritoryPart>();
            if (hierarchyPart == null || territoryPart == null) {
                return HttpNotFound();
            }

            #region Authorize
            ActionResult redirectTo;
            if (ShouldRedirectForPermissions(hierarchyPart.Record.Id, out redirectTo)) {
                return redirectTo;
            }
            foreach (var permission in context.AdditionalPermissions) {
                if (!_authorizer.Authorize(permission, context.TerritoryItem, context.Message))
                    return new HttpUnauthorizedResult();
            }
            #endregion

            return context.ExecutionAction(context.TerritoryItem);
        }

        /// <summary>
        /// This method performs a bunch of default checks to verify that the user is allowed to proceed
        /// with the action it called. This will return false if the user is authorized to proceed.
        /// </summary>
        /// <param name="hierarchyId">The Id of a hierarchy ContentItem.</param>
        /// <returns>Returns false if the caller is authorized to proceed. Otherwise the ou ActionResult
        /// argument is populated with the Action the user should be redirected to.</returns>
        private bool ShouldRedirectForPermissions(int hierarchyId, out ActionResult redirectTo) {
            redirectTo = null;
            if (AllowedHierarchyTypes == null) {
                redirectTo = new HttpUnauthorizedResult(TerritoriesUtilities.Default401HierarchyMessage);
                return true;
            }
            if (AllowedTerritoryTypes == null) {
                redirectTo = new HttpUnauthorizedResult(TerritoriesUtilities.Default401TerritoryMessage);
                return true;
            }

            var hierarchyItem = _contentManager.Get(hierarchyId, VersionOptions.Latest);
            if (hierarchyItem == null) {
                redirectTo = HttpNotFound();
                return true;
            }
            var hierarchyPart = hierarchyItem.As<TerritoryHierarchyPart>();
            if (hierarchyPart == null) {
                redirectTo = HttpNotFound();
                return true;
            }

            if (!AllowedHierarchyTypes.Any(ty => ty.Name == hierarchyItem.ContentType)) {
                var typeName = _contentDefinitionManager.GetTypeDefinition(hierarchyItem.ContentType).DisplayName;
                redirectTo = new HttpUnauthorizedResult(TerritoriesUtilities.SpecificHierarchy401Message(typeName));
                return true;
            }
            if (!AllowedTerritoryTypes.Any(ty => ty.Name == hierarchyPart.TerritoryType)) {
                var typeName = _contentDefinitionManager.GetTypeDefinition(hierarchyPart.TerritoryType).DisplayName;
                redirectTo = new HttpUnauthorizedResult(TerritoriesUtilities.SpecificTerritory401Message(typeName));
                return true;
            }

            return false;
        }

        private Lazy<IEnumerable<ContentTypeDefinition>> _allowedTerritoryTypes;
        private IEnumerable<ContentTypeDefinition> AllowedTerritoryTypes {
            get { return _allowedTerritoryTypes.Value; }
        }

        /// <summary>
        /// This method gets all the territory types the current user is allowed to manage.
        /// </summary>
        /// <returns>Returns the types the user is allowed to manage. Returns null if the user lacks the correct 
        /// permissions to be invoking these actions.</returns>
        private IEnumerable<ContentTypeDefinition> GetAllowedTerritoryTypes() {
            var allowedTypes = _territoriesService.GetTerritoryTypes();
            if (!allowedTypes.Any() && //no dynamic permissions
                !_authorizer.Authorize(TerritoriesPermissions.ManageTerritories)) {

                return null;
            }

            return allowedTypes;
        }

        private Lazy<IEnumerable<ContentTypeDefinition>> _allowedHierarchyTypes;
        private IEnumerable<ContentTypeDefinition> AllowedHierarchyTypes {
            get { return _allowedHierarchyTypes.Value; }
        }

        /// <summary>
        /// This method gets all the hierarchy types the current user is allowed to manage.
        /// </summary>
        /// <returns>Returns the types the user is allwoed to manage. Returns null if the user lacks the correct 
        /// permissions to be invoking these actions.</returns>
        private IEnumerable<ContentTypeDefinition> GetAllowedHierarchyTypes() {
            var allowedTypes = _territoriesService.GetHierarchyTypes();
            if (!allowedTypes.Any() && //no dynamic permissions
                !_authorizer.Authorize(TerritoriesPermissions.ManageTerritoryHierarchies)) {

                return null;
            }

            return allowedTypes;
        }

        private TerritoryHierarchyTreeNode MakeANode(TerritoryPart territoryPart) {
            var metadata = _contentManager.GetItemMetadata(territoryPart.ContentItem);
            var requestContext = _workContextAccessor.GetContext().HttpContext.Request.RequestContext;
            return new TerritoryHierarchyTreeNode {
                Id = territoryPart.ContentItem.Id,
                EditUrl = _routeCollection.GetVirtualPath(requestContext, metadata.EditorRouteValues).VirtualPath,
                DisplayText = metadata.DisplayText
            };
        }
        
        #region IUpdateModel implementation
        public void AddModelError(string key, LocalizedString errorMessage) {
            ModelState.AddModelError(key, errorMessage.ToString());
        }

        public void AddModelError(string key, string errorMessage) {
            ModelState.AddModelError(key, errorMessage);
        }

        bool IUpdateModel.TryUpdateModel<TModel>(TModel model, string prefix, string[] includeProperties, string[] excludeProperties) {
            return TryUpdateModel(model, prefix, includeProperties, excludeProperties);
        }
        #endregion
    }
}
