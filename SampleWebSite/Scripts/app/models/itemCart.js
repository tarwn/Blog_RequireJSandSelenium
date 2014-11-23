define(["knockout"],
function (ko) {

    function ItemCart(itemSummary, count) {
        this._itemSummary = itemSummary;

        this.count = ko.observable(count);

        this.id = ko.computed(function () {
            return this._itemSummary.id();
        }, this);

        this.name = ko.computed(function () {
            return this._itemSummary.name();
        }, this);

        this.price = ko.computed(function () {
            return this._itemSummary.price();
        }, this);

        this.subTotal = ko.computed(function () {
            return this._itemSummary.price() * this.count();
        }, this);

    }

    ItemCart.prototype.toString = function toString() {
        return '[object ItemCart { id: "' + this.id() + '" ... }';
    };

    return ItemCart;

});