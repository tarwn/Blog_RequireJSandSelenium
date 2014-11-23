define(["knockout"],
function (ko) {

    function ItemSummary(rawData) {
        this.id = ko.observable(rawData.id || "new");
        this.name = ko.observable(rawData.name || "new item");
        this.inStock = ko.observable(rawData.inStock || 0);
        this.price = ko.observable(rawData.price || 0);
    }

    ItemSummary.prototype.toString = function toString() {
        return '[object ItemSummary { id: "' + this.id() + '" ... }';
    };

    return ItemSummary;

});