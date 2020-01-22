var calculator = new Vue({
    el: '#calculator',
    data: {
        search: "",
        timeout: null,
        searchResult: [],
        categories: [],
        products: [],
        selectedProducts: []
    },
    watch: {
        search: function () {
            var self = this;

            if (self.timeout !== null)
                clearTimeout(self.timeout);

            if (self.search.length > 2) {
                // debugger;
                self.timeout = setTimeout(function () {
                    console.log("about to search");
                    $.ajax({
                        url: '/Shop/SearchProducts?search=' + self.search,
                        success: function (data, textStatus, jqXHR) {
                            var linq = Enumerable.From(data);
                            var linqResult = linq.GroupBy(function (x) { return x.tertiaryCategory; })
                                .Select(function (x) {
                                    return {
                                        key: x.Key(),
                                        searchResult: x.source
                                    };
                                }).ToArray();

                            console.log("search results mapped");
                            self.searchResult = linqResult;
                            console.log(self.searchResult[0]);
                        }
                    });
                }.bind(this), 500);
            }
        }
    },
    created() {
        this.loadCategories();
        //this.loadProducts("", 0);
    },
    methods: {
        loadCategories() {
            axios
                .get('/Shop/Categories')
                .then(res => {
                    this.categories = res.data;
                });
        },
        loadProducts(cat, catId) {
            var ext = "";
            if (cat.length > 0) {
                ext = "?category=" + cat + "&categoryId=" + catId;
            }
            axios
                .get('/Shop/Products' + ext)
                .then(res => {
                    this.products = res.data.map(x => {
                        return {
                            ...x,
                            qty: 1
                        };
                    });
                });
            this.searchResult = [];
        },
        selectProduct(p) {
            var isUnique = true;

            for (var i = 0; i < this.selectedProducts.length; i++) {
                if (p.id === this.selectedProducts[i].id) {
                    isUnique = false;
                }
            }

            if (isUnique) {
                var qty = p.qty === undefined ? 1 : p.qty;
                var product = {
                    id: p.id,
                    code: p.code,
                    name: p.name,
                    colour: p.colour,
                    uom: p.uom,
                    qty: qty,
                    price: parseFloat(p.price.replace('R', '').replace(',', '.')).toFixed(2),
                    //price: parseFloat(p.price.replace('R', '').replace(',', '')).toFixed(2),
                    value: p.value,
                    discount: 0.0,
                    competitorPrice: 0.0,
                    valueAdded: p.valueAdded,
                    classValue: (p.valueAdded === true) ? "green-box" : ""
                };
                $(".empty-table").hide();

                this.selectedProducts.push(product);
                this.searchResult = [];
            }
        },
        getDiscount(p) {
            if (p.discount > 0) {
                return p.price * ((100 - p.discount) / 100);
            } else {
                return p.price;
            }
        },
        getOurTotal(p) {
            return (p.qty * this.getDiscount(p)).toFixed(2);
        },
        getCompTotal(p) {
            return (p.qty * p.competitorPrice).toFixed(2);
        },
        getSavings(p) {
            return (this.getCompTotal(p) - this.getOurTotal(p)).toFixed(2);
        },
        getSavingsPercent(p) {
            if (this.getCompTotal(p) <= 0) return 0;
            return (100 - (this.getOurTotal(p) / this.getCompTotal(p)) * 100).toFixed(2);
        },
        updateQtyUp(p) {
            for (var i = 0; i < this.selectedProducts.length; i++) {
                if (this.selectedProducts[i].id === p.id) {
                    this.selectedProducts[i].qty = this.selectedProducts[i].qty + 1;
                }
            }
        },
        updateQtyDown(p) {
            for (var i = 0; i < this.selectedProducts.length; i++) {
                if (this.selectedProducts[i].id === p.id) {
                    if (this.selectedProducts[i].qty > 1) {
                        this.selectedProducts[i].qty = this.selectedProducts[i].qty - 1;
                    }
                    else {
                        this.selectedProducts.splice(i, 1);
                    }
                }
            }
        }
    },
    computed: {
        ourTotal() {
            if (this.selectedProducts.length === 0) return 0;
            return this.selectedProducts
                .map(x => parseFloat(this.getOurTotal(x)))
                .reduce((x, y) => x + y, 0).toFixed(2);
        },
        compTotal() {
            if (this.selectedProducts.length === 0) return 0;
            return this.selectedProducts
                .map(x => parseFloat(this.getCompTotal(x)))
                .reduce((x, y) => x + y, 0).toFixed(2);
        },
        savingsTotal() {
            if (this.selectedProducts.length === 0) return 0;
            return this.selectedProducts
                .map(x => parseFloat(this.getSavings(x)))
                .reduce((x, y) => x + y, 0).toFixed(2);
        },
        savingsPercentTotal() {
            if (this.selectedProducts.length === 0) return 0;
            return this.selectedProducts
                .map(x => parseFloat(this.getSavingsPercent(x)))
                .reduce((x, y) => x + y, 0).toFixed(2);
        }
    }
});