$(document).ready(function () {
    $("#search-navbar").css('display', 'block');
    var search = new Vue({
        el: '#search',
        data: {

            search: "",
            timeout: null,
            products: []

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
                                            products: x.source
                                        };
                                    }).ToArray();

                                self.products = linqResult;
                            }
                        });
                    }.bind(this), 500);
                }
            }
        }
    });
});