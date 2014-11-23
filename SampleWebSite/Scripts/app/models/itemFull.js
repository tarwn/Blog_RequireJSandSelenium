define(["knockout"],
function (ko) {

    function ItemFull(rawData) {
        this.id = ko.observable(rawData.id || "new");
        this.name = ko.observable(rawData.name || "new item");
        this.inStock = ko.observable(rawData.inStock || 0);
        this.price = ko.observable(rawData.price || 0);
        this.summary = ko.observable(rawData.summary || "this is a new product");
    }

    ItemFull.prototype.toString = function toString() {
        return '[object ItemFull { id: "' + this.id() + '" ... }';
    };

    return ItemFull;

});