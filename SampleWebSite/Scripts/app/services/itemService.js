define(["jQuery", "lodash"],
function ($, _) {

    function ItemService() {
        this.fakeServiceResults = [
            { id: 1, name: "One", inStock: 5, price: 5.50, summary: "product one" },
            { id: 2, name: "product two", inStock: 1, price: 6.25, summary: "product two" },
            { id: 3, name: "product three", inStock: 2, price: 2.50, summary: "product three" },
            { id: 4, name: "product four", inStock: 0, price: 7.00, summary: "product four" }
        ];
    };

    ItemService.prototype.search = function (searchText) {
        console.log('ItemService.search("' + searchText + '")');

        return this.fakeServiceResults;
    };

    ItemService.prototype.getItem = function (itemId) {
        console.log('ItemService.getItem("' + itemId + '")');

        return _.find(this.fakeServiceResults, function (item) {
            return item.id == itemId;
        });
    }

    return ItemService;

});