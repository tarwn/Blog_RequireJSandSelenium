define(["knockout", "lodash"],
function (ko, _) {

    function Cart() {
        this.contents = ko.observableArray([]);
        
        this.count = ko.computed(function () {
            return _.reduce(this.contents(), function (runningSum, item) {
                return runningSum + item.count();
            }, 0);
        }, this);

        this.subTotal = ko.computed(function () {
            return _.reduce(this.contents(), function (runningSum, item) {
                return runningSum + item.subTotal();
            }, 0);
        }, this);
    }

    return Cart;

});