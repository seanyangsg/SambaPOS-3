﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Linq;
using Samba.Domain.Models.Menus;
using Samba.Domain.Models.Tickets;
using Samba.Localization.Properties;
using Samba.Presentation.Common;
using Samba.Presentation.Common.ModelBase;
using Samba.Presentation.ViewModels;
using Samba.Services;

namespace Samba.Modules.TicketModule
{
    [Export, PartCreationPolicy(CreationPolicy.NonShared)]
    public class OrderTagGroupViewModel : EntityViewModelBase<OrderTagGroup>
    {
        private readonly IMenuService _menuService;
        public MapController<OrderTagMap, OrderTagMapViewModel> MapController { get; set; }

        [ImportingConstructor]
        public OrderTagGroupViewModel(IMenuService menuService, IUserService userService, IDepartmentService departmentService, ISettingService settingService)
        {
            _menuService = menuService;
            AddOrderTagCommand = new CaptionCommand<string>(string.Format(Resources.Add_f, Resources.OrderTag), OnAddPropertyExecuted);
            DeleteOrderTagCommand = new CaptionCommand<string>(string.Format(Resources.Delete_f, Resources.OrderTag), OnDeletePropertyExecuted, CanDeleteProperty);
        }

        private ObservableCollection<OrderTagViewModel> _orderTags;
        public ObservableCollection<OrderTagViewModel> OrderTags { get { return _orderTags ?? (_orderTags = new ObservableCollection<OrderTagViewModel>(GetOrderTags(Model))); } }

        private readonly IList<string> _selectionTypes = new[] { string.Format(Resources.Selection_f, Resources.Multiple), string.Format(Resources.Selection_f, Resources.Single) };
        public IList<string> SelectionTypes { get { return _selectionTypes; } }

        public ICaptionCommand AddOrderTagCommand { get; set; }
        public ICaptionCommand DeleteOrderTagCommand { get; set; }

        public string ButtonHeader { get { return Model.ButtonHeader; } set { Model.ButtonHeader = value; } }
        public bool AddTagPriceToOrderPrice { get { return Model.AddTagPriceToOrderPrice; } set { Model.AddTagPriceToOrderPrice = value; } }
        public int ButtonHeight { get { return Model.ButtonHeight; } set { Model.ButtonHeight = value; } }
        public int ColumnCount { get { return Model.ColumnCount; } set { Model.ColumnCount = value; } }
        public int TerminalButtonHeight { get { return Model.TerminalButtonHeight; } set { Model.TerminalButtonHeight = value; } }
        public int TerminalColumnCount { get { return Model.TerminalColumnCount; } set { Model.TerminalColumnCount = value; } }
        public string SelectionType { get { return SelectionTypes[Model.SelectionType]; } set { Model.SelectionType = SelectionTypes.IndexOf(value); } }
        public bool UnlocksOrder { get { return Model.UnlocksOrder; } set { Model.UnlocksOrder = value; } }
        public bool CalculateOrderPrice { get { return Model.CalculateOrderPrice; } set { Model.CalculateOrderPrice = value; } }
        public bool DecreaseOrderInventory { get { return Model.DecreaseOrderInventory; } set { Model.DecreaseOrderInventory = value; } }

        public OrderTagViewModel SelectedOrderTag { get; set; }

        private void OnDeletePropertyExecuted(string obj)
        {
            if (SelectedOrderTag == null) return;
            if (SelectedOrderTag.Model.Id > 0)
                Workspace.Delete(SelectedOrderTag.Model);
            Model.OrderTags.Remove(SelectedOrderTag.Model);
            OrderTags.Remove(SelectedOrderTag);
        }

        private bool CanDeleteProperty(string arg)
        {
            return SelectedOrderTag != null;
        }

        private void OnAddPropertyExecuted(string obj)
        {
            OrderTags.Add(new OrderTagViewModel(MenuItem.AddDefaultMenuItemProperty(Model), _menuService));
        }

        private IEnumerable<OrderTagViewModel> GetOrderTags(OrderTagGroup baseModel)
        {
            return baseModel.OrderTags.Select(item => new OrderTagViewModel(item, _menuService));
        }

        public override string GetModelTypeString()
        {
            return Resources.OrderTagGroup;
        }

        public override Type GetViewType()
        {
            return typeof(OrderTagGroupView);
        }

        protected override void Initialize()
        {
            base.Initialize();
            MapController = new MapController<OrderTagMap, OrderTagMapViewModel>(Model.OrderTagMaps, Workspace);
        }
    }
}
