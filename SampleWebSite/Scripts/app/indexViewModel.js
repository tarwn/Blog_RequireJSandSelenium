define(["knockout", 
        "lodash",
        "app/services/itemService", 
        "app/models/itemSummary",
        "app/models/itemFull",
        "app/models/itemCart",
        "app/models/cart"
],
function (ko,
        _,
        ItemService,
        ItemSummary,
        ItemFull,
        ItemCart,
        Cart) {
   
    function IndexViewModel() {
        var itemService = new ItemService();
        var self = this;

        this.searchText = ko.observable();
        this.isSearching = ko.observable(false);
        this.searchResults = ko.observableArray();
        this.executeSearch = function () {
            console.log('IndexViewModel.executeSearch("' + searchText + '");')

            //clear current search selection
            self.selectedItem(null);

            // perform the search
            self.isSearching(true);
            var searchText = self.searchText();

            itemService.search(searchText).then(function (rawResults) {
                // convert to models and display
                var results = _.map(rawResults, function (rawResult) {
                    return new ItemSummary(rawResult);
                });
                self.searchResults(results);
                self.isSearching(false);
            });
        };
        
        this.selectedItem = ko.observable();
        this.isLoadingSelectedItem = ko.observable(false);
        this.selectItem = function (item) {
            console.log('IndexViewModel.selectItem(' + item + ');')

            self.isLoadingSelectedItem(true);
            itemService.getItem(item.id()).then(function (rawFullItem) {
                var fullItem = new ItemFull(rawFullItem);
                self.selectedItem(fullItem);
                self.isLoadingSelectedItem(false);
            });
        };

        this.cart = new Cart();
        this.addToCart = function (itemToAdd) {
            console.log('IndexViewModel.addToCart(' + itemToAdd + ');')

            var selectionId = itemToAdd.id();
            var existingCartItem = _.find(self.cart.contents(), function (cartItem) {
                return selectionId == cartItem.id();
            });

            if (existingCartItem != null) {
                existingCartItem.count(existingCartItem.count() + 1);
            }
            else {
                var cartItem = new ItemCart(itemToAdd, 1);
                self.cart.contents.push(cartItem);
            }
        };
    }

    return IndexViewModel;

});