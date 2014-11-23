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
        this.searchResults = ko.observableArray();
        this.executeSearch = function () {
            console.log('IndexViewModel.executeSearch("' + searchText + '");')

            //clear current search selection
            self.selectedItem(null);

            // peform the search
            var searchText = self.searchText();
            var rawResults = itemService.search(searchText);

            // convert to models and display
            var results = _.map(rawResults, function (rawResult) {
                return new ItemSummary(rawResult);
            });
            self.searchResults(results);
        };
        
        this.selectedItem = ko.observable();
        this.selectItem = function (item) {
            console.log('IndexViewModel.selectItem(' + item + ');')

            var rawFullItem = itemService.getItem(item.id());
            var fullItem = new ItemFull(rawFullItem);
            self.selectedItem(fullItem);
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