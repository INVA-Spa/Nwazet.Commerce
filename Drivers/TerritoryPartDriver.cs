﻿using Nwazet.Commerce.Models;
using Orchard.ContentManagement.Drivers;
using Orchard.Environment.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;
using Orchard.ContentManagement;
using Orchard;
using Nwazet.Commerce.Services;
using Orchard.UI.Notify;
using Orchard.Localization;
using Nwazet.Commerce.ViewModels;

namespace Nwazet.Commerce.Drivers {
    [OrchardFeature("Territories")]
    public class TerritoryPartDriver : ContentPartDriver<TerritoryPart> {

        private readonly IWorkContextAccessor _workContextAccessor;
        private readonly ITerritoriesService _territoriesService;
        private readonly INotifier _notifier;
        private readonly IContentManager _contentManager;
        private readonly ITerritoriesRepositoryService _territoriesRepositoryService;

        public TerritoryPartDriver(
            IWorkContextAccessor workContextAccessor,
            ITerritoriesService territoriesService,
            INotifier notifier,
            IContentManager contentManager,
            ITerritoriesRepositoryService territoriesRepositoryService) {

            _workContextAccessor = workContextAccessor;
            _territoriesService = territoriesService;
            _notifier = notifier;
            _contentManager = contentManager;
            _territoriesRepositoryService = territoriesRepositoryService;

            T = NullLocalizer.Instance;
        }

        public Localizer T;

        protected override string Prefix {
            get { return "TerritoryPart"; }
        }

        protected override DriverResult Editor(TerritoryPart part, dynamic shapeHelper) {
            
            var shapes = new List<DriverResult>();
            //part.id == 0: new item
            if (part.Id == 0 || part.Record.Hierarchy == null) {
                shapes.AddRange(CreationEditor(part, shapeHelper));
            } else {
                shapes.AddRange(ProperEditor(part, shapeHelper));
            }
            
            return Combined(shapes.ToArray());
        }

        private IEnumerable<DriverResult> CreationEditor(TerritoryPart part, dynamic shapeHelper) {
            int hierarchyId;
            var shapes = new List<DriverResult>();
            // we don't know the Hierarchy for this territory here, so we try to get it from
            // the request path (in case we are creating a territory from the HierarchyTerritoriesAdminController
            if (!TryValidateCreationContext(out hierarchyId)) {
                hierarchyId = 0;
            }
            List<TerritoryInternalRecord> territoryInternals;
            if (hierarchyId == 0) {
                // no valid hierarchy means we cannot have a valid list of allowed
                // TerritoryInternalRecord
                InvalidHierarchyOnCreation();
            } else {
                // We have a hierarchy that we can use to validate a list of allowed
                // TerritoryInternalRecord
                var hierarchy = _contentManager.Get<TerritoryHierarchyPart>(hierarchyId);
                if (hierarchy == null) {
                    // We don't really have a hierarchy after all
                    InvalidHierarchyOnCreation();
                } else {
                    // Healthy situation
                    territoryInternals = _territoriesService.GetAvailableTerritoryInternals(hierarchy).ToList();
                    if (territoryInternals.Any()) {
                        // There are TerritoryInternalRecords we can pick from
                        shapes.Add(ContentShape("Parts_TerritoryPart_Creation",
                            () => shapeHelper.EditorTemplate(
                                TemplateName: "Parts/TerritoryPartCreation",
                                Model: new TerritoryPartViewModel() {
                                    AvailableTerritoryInternalRecords = territoryInternals,
                                    Hierarchy = hierarchy
                                },
                                Prefix: Prefix
                                )));
                    } else {
                        // There is no TerritoryInternalRecord available
                        // This is also verified in the HierarchyTerritoriesAdminController call. However, something
                        // has clearly happened in the meanwhile.
                        _notifier.Error(T("There are no territories that may be added to hierarchy. Content creation will fail."));
                    }
                }
            }

            return shapes;
        }

        private void InvalidHierarchyOnCreation() {
            InvalidHierarchyNotification(T("Content creation"));
        }

        private IEnumerable<DriverResult> ProperEditor(TerritoryPart part, dynamic shapeHelper) {

            var shapes = new List<DriverResult>();

            // The territory here must exist in a hierarchy and with a selected unique record.
            var territoryInternals = _territoriesService
                .GetAvailableTerritoryInternals(part.HierarchyPart)
                .ToList();
            var model = new TerritoryPartViewModel() {
                AvailableTerritoryInternalRecords = territoryInternals,
                Hierarchy = part.HierarchyPart,
                Parent = part.ParentPart,
                Part = part
            };
            model.AvailableTerritoryInternalRecords.Add(part.Record.TerritoryInternalRecord);

            shapes.Add(ContentShape("Parts_TerritoryPart_Edit",
                () => shapeHelper.EditorTemplate(
                    TemplateName: "Parts/TerritoryPartEdit",
                    Model: model,
                    Prefix: Prefix
                    )));

            return shapes;
        }

        private void InvalidHierarchyOnEdit() {
            InvalidHierarchyNotification(T("Content edit"));
        }

        private void InvalidHierarchyNotification(LocalizedString detail) {
            _notifier.Error(InvalidHierarchyErrorMessage(detail));
        }

        private LocalizedString InvalidHierarchyErrorMessage(LocalizedString detail = null) {
            if (detail != null) {
                return T("Impossible to identify a valid Hierarchy for this territory. {0} will fail.", detail);
            }
            return T("Impossible to identify a valid Hierarchy for this territory.");
        }

        protected override DriverResult Editor(TerritoryPart part, IUpdateModel updater, dynamic shapeHelper) {

            var viewModel = new TerritoryPartViewModel();
            if (updater.TryUpdateModel(viewModel, Prefix, null, null)) {
                var hierarchy = part.HierarchyPart;
                if (hierarchy == null) {
                    int hierarchyId;
                    if (!TryValidateCreationContext(out hierarchyId)) {
                        hierarchyId = 0;
                    }
                    hierarchy = _contentManager.Get<TerritoryHierarchyPart>(hierarchyId);
                }
                if (hierarchy == null) {
                    updater.AddModelError("Hierarchy", InvalidHierarchyErrorMessage());
                } else {
                    var avalaibleInternals = _territoriesService.GetAvailableTerritoryInternals(hierarchy);
                    int selectedId;
                    if (int.TryParse(viewModel.SelectedRecordId, out selectedId)) {
                        var selectedRecord = _territoriesRepositoryService.GetTerritoryInternal(selectedId);
                        if (selectedRecord == null) {
                            updater.AddModelError("Territory", InvalidInternalRecordMessage);
                        } else {
                            if (part.Record.TerritoryInternalRecord != null && part.Record.TerritoryInternalRecord.Id == selectedId) {
                                // nothing to do here, right?
                            } else {
                                var fromAvailables = avalaibleInternals.FirstOrDefault(tir => tir.Id == selectedId);
                                if (fromAvailables == null) {
                                    updater.AddModelError("Territory", InvalidInternalRecordMessage);
                                } else {
                                    part.Record.TerritoryInternalRecord = fromAvailables;
                                }
                            }
                        }
                    }
                }

            }

            return Editor(part, shapeHelper);
        }

        private LocalizedString InvalidInternalRecordMessage {
            get { return T("Invalid territory record."); }
        }

        private bool TryValidateCreationContext(out int hierarchyId) {
            hierarchyId = 0;
            var request = _workContextAccessor.GetContext()
                    .HttpContext.Request;
            var routeValues = request.RequestContext.RouteData.Values;
            if (routeValues.ContainsKey("area") &&
                routeValues.ContainsKey("controller") &&
                routeValues.ContainsKey("action") &&
                routeValues["area"].ToString().Equals("Nwazet.Commerce", StringComparison.OrdinalIgnoreCase) &&
                routeValues["controller"].ToString().Equals("HierarchyTerritoriesAdmin", StringComparison.OrdinalIgnoreCase) &&
                routeValues["action"].ToString().Equals("CreateTerritory", StringComparison.OrdinalIgnoreCase) &&
                request.QueryString.AllKeys.Any(k => k.Equals("hierarchyId", StringComparison.OrdinalIgnoreCase))) {

                if (int.TryParse(request.QueryString["hierarchyId"], out hierarchyId)) {
                    return true;
                }
            }

            return false;
        }
        
    }
}