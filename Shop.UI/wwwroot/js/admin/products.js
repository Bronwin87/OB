var baseProductModal = function () {
    return {
        id: 0,
        name: "Product Name",
        description: "Product Description",
        value: 1.99,
        published: false,
        outOfStock: false,
        noDiscount: false,
        valueAdd: false,
        searchString: "Undefined",
        costPrice: 1.99,
        externalId: "Undefined",
        unit: "Each",
        colour: "Undefined",
        imageUrl: "Undefined"
    };
};

var baseModal = function () {
    return {
        active: false,
        link: false,
        alt: false,
        valueAdded: false,
        products: [],
        search: "",
        loading: false,
        timeout: null
    };
};

var app = new Vue({
    el: '#app',
    data: {
        editing: false,
        loading: false,
        search: {
            text: "",
            timeout: null
        },
        table: {
            page: 1,
            perPage: 25,
            total: 0,
            loading: false
        },
        productModel: baseProductModal(),
        addProductModal: baseModal(),
        categories: [],
        products: [],
        tempFile: null,
        tempImg: ""
    },
    watch: {
        'search.text': function () {
            var self = this;

            if (self.search.timeout !== null)
                clearTimeout(self.search.timeout);

            self.search.timeout = setTimeout(function () {
                self.page = 1;
                self.total = 0;
                self.getProducts();
            }, 500);
        },
        'addProductModal.active': function (v) {
            if (!v)
                this.addProductModal = baseModal();
        },
        'addProductModal.search': function (v) {
            if (v === '') {
                this.addProductModal.products = [];
                return;
            }
            var self = this;

            if (self.addProductModal.timeout !== null)
                clearTimeout(self.addProductModal.timeout);

            self.addProductModal.timeout = setTimeout(function () {
                axios.get('/products?page=0'
                    + '&perPage=0'
                    + '&needPublish=true'
                    + '&valueAdded=' + self.addProductModal.valueAdded
                    + '&search=' + self.addProductModal.search)
                    .then(res => {
                        self.addProductModal.products = res.data.products;
                    });
            }, 500);
        }
    },
    mounted() {
        this.loadCategories();
        this.getProducts();
    },
    methods: {
        getProduct(id) {
            this.loading = true;
            axios.get('/products/' + id)
                .then(res => {
                    this.productModel = res.data;
                })
                .catch(err => {
                    console.log(err);
                })
                .then(() => {
                    this.loading = false;
                });
        },
        loadCategories() {
            axios.post('/products/cat', null)
                .then(res => {
                    this.categories = res.data;
                });
        },
        getProducts() {
            this.table.loading = true;
            axios.get('/products?page=' + this.table.page
                + '&perPage=' + this.table.perPage
                + '&search=' + this.search.text)
                .then(res => {
                    this.products = res.data.products;
                    this.table.total = res.data.total;
                })
                .catch(err => {
                    console.log(err);
                })
                .then(() => {
                    this.table.loading = false;
                });
        },
        onPageChange(page) {
            this.table.loading = true;
            this.table.page = page;
            this.getProducts();
        },
        createProduct() {
            this.loading = true;

            var data = new FormData();

            data.append('request.name', this.productModel.name);
            data.append('request.description', this.productModel.description);
            data.append('request.searchString', this.productModel.searchString);
            data.append('request.colour', this.productModel.colour);
            data.append('request.unit', this.productModel.unit);
            data.append('request.externalId', this.productModel.externalId);

            data.append('request.published', this.productModel.published);
            data.append('request.outOfStock', this.productModel.outOfStock);
            data.append('request.noDiscount', this.productModel.noDiscount);

            data.append('request.main', this.productModel.main);
            data.append('request.sub', this.productModel.sub);
            data.append('request.tri', this.productModel.tri);

            data.append('request.value', this.productModel.value);
            data.append('request.costPrice', this.productModel.costPrice);

            data.append('request.file', this.tempFile);

            axios.post('/products', data)
                .then(res => {
                    this.productModel = res.data;
                })
                .catch(err => {
                    console.log(err);
                })
                .then(() => {
                    this.loading = false;
                });
        },
        updateProduct() {
            this.loading = true;

            var data = new FormData();

            data.append('request.id', this.productModel.id);
            data.append('request.name', this.productModel.name);
            data.append('request.description', this.productModel.description);
            data.append('request.searchString', this.productModel.searchString);
            data.append('request.colour', this.productModel.colour);
            data.append('request.unit', this.productModel.unit);
            data.append('request.externalId', this.productModel.externalId);

            data.append('request.published', this.productModel.published);
            data.append('request.outOfStock', this.productModel.outOfStock);
            data.append('request.noDiscount', this.productModel.noDiscount);

            data.append('request.main', this.productModel.main);
            data.append('request.sub', this.productModel.sub);
            data.append('request.tri', this.productModel.tri);

            data.append('request.value', this.productModel.value);
            data.append('request.costPrice', this.productModel.costPrice);

            data.append('request.file', this.tempFile);

            axios.put('/products/' + this.productModel.id, data)
                .then(res => {
                    this.getProducts();
                })
                .catch(err => {
                    console.log(err);
                })
                .then(() => {
                    this.loading = false;
                    this.editing = false;
                });
        },
        deleteProduct(id, index) {
            this.loading = true;
            axios.delete('/products/' + id)
                .then(res => {
                    console.log(res);
                    this.products.splice(index, 1);
                })
                .catch(err => {
                    console.log(err);
                })
                .then(() => {
                    this.loading = false;
                });
        },
        newProduct() {
            this.editing = true;
            this.productModel = baseProductModal();
            this.tempFile = null;
            this.tempImg = "";
        },
        editProduct(row) {
            this.getProduct(row.id);
            this.editing = true;
            this.tempFile = null;
            this.tempImg = "";
        },
        cancel() {
            this.editing = false;
            this.productModel = baseProductModal();
        },
        selectFile(e) {
            this.tempFile = e.target.files[0];
            this.tempImg = URL.createObjectURL(this.tempFile);
        },
        clearFile() {
            this.tempFile = null;
            this.tempImg = "";
        },
        openModal(type) {
            this.addProductModal.active = true;
            this.addProductModal[type] = true;
            this.addProductModal.products = [];
            this.addProductModal.search = "";
        },
        attachProducts() {
            this.loading = true;

            var selectedProducts = this.addProductModal.products
                .filter(x => x.selected)
                .map(x => x.id);

            axios.post("/products/link", {
                id: this.productModel.id,
                links: selectedProducts,
                link: this.addProductModal.link,
                alt: this.addProductModal.alt,
                value: this.addProductModal.valueAdded
            }).then(res => {
                this.getProduct(this.productModel.id);
                this.addProductModal.active = false;
            });
        },
        removeLink(id) {
            this.loading = true;
            axios.delete("/products/" + this.productModel.id + "/" + id)
                .then(res => {
                    this.getProduct(this.productModel.id);
                });
        }
    },
    computed: {
        subCat() {
            var sub = this.categories
                .filter(x => x.id === this.productModel.main)[0];

            if (sub !== undefined)
                return sub.categories;
            return [];
        },
        triCat() {
            var tri = this.subCat
                .filter(x => x.id === this.productModel.sub)[0];
            if (tri !== undefined)
                return tri.categories;
            return [];
        }
    }
});