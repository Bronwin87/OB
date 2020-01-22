var officebox = {
    version : '0.0.1',
    showSubmit : 'false',
    checkShowSubmit : function(class_selector) {
        officebox.showSubmit = 'false';
        $(class_selector).each(function(index) {
            if ($(this).attr('checked') == 'checked') {
                officebox.showSubmit = 'true';
            }
        });
    },

    recalculatePrice : function(product_id, index, quantity, educationalinstitution_id) {
        $('.schoolproduct-price-' + index).html('<img src="' + site_url + 'images/ajax-loader.gif" />');
        $.ajax({
            type : "GET",
            url : site_url + "index.php",
            data : {
                action : 'calculateproductprice',
                id : product_id,
                quantity : quantity,
                educationalinstitution_id : educationalinstitution_id
            },
            dataType : 'json'
        }).done(function(price) {
            if ($('.schoolproduct-price-' + index).html() != price.price) {
                $('.schoolproduct-price-' + index).html(price.price);
            }
            if (typeof officebox.recalculateTotalPrices === "function") {
                officebox.recalculateTotalPrices();
            }
        });
    },

    recalculateTotalPrices : function() {
        var product_quantities = {};
        var products = [];
        
        $('.product-ids').empty();
        
        $('.subject-product-select').each(function(index) {
            product_id = $(this).val();
            if ($(this).attr('checked') == 'checked') {
                new_quantity = parseInt($(this).data('quantity'));
            } else {
                new_quantity = 0;
            }

            console.log(product_id);
            console.log(new_quantity);
            
            if (product_quantities.hasOwnProperty(product_id)){
                product_quantities[product_id] = product_quantities[product_id] + new_quantity;
            }else {
                product_quantities[product_id] = new_quantity;
            }
            products.push({
                id : product_id,
                quantity : new_quantity
            });
        });
        $('.net-price').html('Total: ');
        $('.tax-price').html('VAT: ');
        $('.total-price').html('<em>Total Price:</em> ' + '<img src="' + site_url + 'images/ajax-loader.gif" />');
        $.ajax({
            type : "GET",
            url : site_url + "index.php",
            data : {
                action : 'calculateorderprice',
                products : products,
                educationalinstitution_id : $('#add-cart-items').data('educationalinstitution_id')
            },
            dataType : 'json'
        }).done(function(price) {
            
            $.each( product_quantities, function( id, quantity ) {
                var input = '.hidden-quantity-input-' + id;
                $(input).val(quantity);
                if (quantity > 0){
                    html = $('.product-ids').append('<input class="hidden-product-input-' + id + '" name="product_id[]" value="' + id + '" type="hidden">');
                }
            });
            $('.net-price').html('Total: ' + price.netprice);
            $('.tax-price').html('VAT: ' + price.taxprice);
            $('.total-price').html('<em>Total Price:</em> ' + price.grossprice);
            
        });
    }
};

(function($) {

    // Work around for jQuery serializeArray method
    $.fn.serializeJSON = function() {
        var json = {};
        $.map($(this).serializeArray(), function(n, i) {
            json[n['name']] = n['value'];
        });
        return json;
    };

})(jQuery);

$(function() {

    $('.formsubmit a').bind('click', function(event) {
        $(this).parent().prev("form").submit();
    });

    $(".rdpform").validate();

    $("#placeorder-form").validate();

    $("#customer-form").validate({
        rules : {
            member_username : {
                required : true,
                minlength : 8
            },
            member_password : {
                required : true,
                minlength : 8
            },
            confirm_password : {
                equalTo : "#member_password"
            }
        }
    });
    $('.add-item-btn').bind('click', function(event) {
        $(this).next('div').children('form').submit();
    });

    $('#register-btn').bind('click', function(event) {
        $("#customer-form").submit();
    });

    $('#change-password-btn').on('click', function(event) {
        $(this).unbind(event);
        $("#facebox").find('#change-password-form').submit();
    });

    $('.remove-item-btn').live('click', function(event) {
        $(this).unbind(event);
        $(this).prev('form').submit();
    });
    $('#ordernow').bind('click', function(event) {
        $('#placeorder-form').submit();
    });
    $('#shoppingcartlink').bind('mouseover', function(event) {
        $('#shoppingcartlink').trigger('click');
    });
    $('#update-cart-link').live('click', function(event) {
        $(this).unbind(event);
        html = '';
        $('#facebox .cms_cart_item_quantity').each(function(index) {
            html = html + '<input type="hidden" name="cms_cart_item_quantity_' + $(this).attr('cartitemid') + '" value="' + $(this).val() + '" />'
        });
        $('#facebox #update-cart-panel').html(html);
        $('#facebox #update-cart-form').submit();
    });
    $('#update-cart-inline-link').on('click', function(event) {
        $(this).unbind(event);
        html = '';
        $('#inline-cart .cms_cart_item_quantity').each(function(index) {
            html = html + '<input type="hidden" name="cms_cart_item_quantity_' + $(this).attr('cartitemid') + '" value="' + $(this).val() + '" />'
        });
        $('#inline-cart #update-cart-panel').html(html);
        $('#inline-cart #update-cart-form').submit();
    });

    $('#approve-order-link').bind('click', function(event) {
        $(this).unbind(event);
        $('#update-order-form #order_state').val('APPROVED');
        $('#update-order-form').submit();
    });

    $('#decline-order-link').bind('click', function(event) {
        $(this).unbind(event);
        $('#update-order-form #order_state').val('CANCELLED');
        $('#update-order-form').submit();
    });

    $('#update-order-link').bind('click', function(event) {
        $(this).unbind(event);
        html = '';
        $('.order_item_quantity').each(function(index) {
            html = html + '<input type="hidden" name="order_item_quantity_' + $(this).attr('orderitemid') + '" value="' + $(this).val() + '" />'
        });

        $('#update-order-panel').html(html);
        $('#update-order-form').attr('action', site_url + 'updateorder/');
        $('#update-order-form').submit();
    });

    $(document).bind('reveal.facebox', function() {
        $('#facebox #submitlogin').on('click', function() {
            $(this).parent('form').submit();
        })

        $('#facebox #forgot-password-link').on('click', function(event) {
            event.preventDefault();
            $('#facebox .content').empty().append('<div class="loading"><img src="' + $.facebox.settings.loadingImage + '"/></div>');
            var link = $(this).attr('href');
            $.get(link, function(data) {
                $("#facebox .content").html(data);
                $('#facebox .content').remove('.loading');
            });
        })

        $('#facebox #submitchangepassword').on('click', function() {
            $(this).parent('form').submit();
        })

        $("#facebox").find('#change-password-form').validate({
            rules : {
                password : {
                    required : true,
                    minlength : 8
                },
                confirm_password : {
                    equalTo : "#password"
                }
            }
        });

        $("#facebox").delegate(".pagination li a", "click", function(event) {
            event.preventDefault();
            if (!$(this).hasClass('selected')) {
                $('#facebox .content').empty().append('<div class="loading"><img src="' + $.facebox.settings.loadingImage + '"/></div>');
                var pagingLink = site_url + $(this).attr('href');
                $.get(pagingLink, function(data) {
                    $("#facebox .content").html(data);
                    $('#facebox .content').remove('.loading');
                });
            }
        });
    });
    $('.productgroupnavigator').find('>ul').addClass('categories').addClass('sf-menu');
    $('.productsubgroupnavigator').find('>ul').addClass('categories');

    $('#productlist-select select').change(function(e) {
        var select = e.target;
        var option = select.options[select.selectedIndex];
        var url = ($('option:selected', this).val());
        if ($(option).hasClass("create-new-list")) {
            jQuery.facebox({
                ajax : url
            });
            $.facebox({
                div : '#newlist-popup'
            }, function() {
                $('#facebox .content').css('width', '345px');
            });
        }
        if ($(option).data('add-item-href') != undefined) {
            window.location = $(option).data('add-item-href');
        }
    });

    $('#btn-submit-createproductlist').live('click', function() {
        $('#createproductlist').submit();
    });

    $('#add-new-list').bind('click', function(event) {
        event.preventDefault();
        jQuery.facebox({
            ajax : $(this).attr('href')
        });
        $.facebox({
            div : '#newlist-popup'
        }, function() {
            $('#facebox .content').css('width', '345px');
        });
    });

    $('.remove-list').live('click', function(event) {
        event.preventDefault();
        $.ajax({
            type : 'POST',
            url : $(this).attr('href'),
            success : function(data) {
                if (data.valid == true) {
                    $.get(site_url + 'productlists/', function(getdata) {
                        $("#productlists").html(getdata);
                    });
                } else {
                    $('#header .wrapper').append('<div id="message"><label class="error">Unable to delete product list.</label></div>');
                    $("html, body").animate({
                        scrollTop : 0
                    });
                }
            },
            error : function(error) {
                $('#header .wrapper').append('<div id="message"><label class="error">An error has occurred, please try again.</label></div>');
                $("html, body").animate({
                    scrollTop : 0
                });
            }
        });
    });

    $('#delete-list').bind('click', function(event) {
        event.preventDefault();
        $.ajax({
            type : 'POST',
            url : $(this).attr('href'),
            success : function(data) {
                if (data.valid == true) {
                    window.location = site_url + 'myaccount/';
                } else {
                    $('#header .wrapper').append('<div id="message"><label class="error">Unable to delete product list.</label></div>');
                    $("html, body").animate({
                        scrollTop : 0
                    });
                }
            },
            error : function(error) {
                $('#header .wrapper').append('<div id="message"><label class="error">An error has occurred, please try again.</label></div>');
                $("html, body").animate({
                    scrollTop : 0
                });
            }
        });
    });

    $(document).bind('reveal.facebox', function() {
        $("#createproductlist").validate({
            submitHandler : function(form) {
                $.ajax({
                    type : 'POST',
                    url : $(form).attr('action'),
                    data : $(form).serializeJSON(),
                    success : function(data) {
                        if (data.valid == true) {
                            $.get(site_url + 'productlists/', function(getdata) {
                                $("#productlists").html(getdata);
                            });
                            $('#productlist-select select').append('<option data-add-item-href="' + site_url + '?action=addcartitem&productlist_id=' + data.id + '&id=' + $('#productlist-select select').data('product-id') + '" value="' + data.id + '">' + data.label + '</option>');
                            $('#productlist-select select').val(data.id).change();
                        } else {
                            $('#header .wrapper').append('<div id="message"><label class="error">Sign in to add a product list.</label></div>');
                            $("html, body").animate({
                                scrollTop : 0
                            });
                            $('#productlist-select select').val('add-to-list').change();
                        }
                    },
                    error : function(error) {
                        $('#header .wrapper').append('<div id="message"><label class="error">An error has occurred, please try again.</label></div>');
                        $("html, body").animate({
                            scrollTop : 0
                        });
                        $('#productlist-select select').val('add-to-list').change();
                    }
                });
                $.facebox.close();
            }
        });
    });

    $('#productlists .pagination ul li a').on('click', function(event) {
        href = $(this).attr('href');
        href = href.replace('action=productlists', 'action=myaccount');
        $(this).attr('href', href);
    });

    $.validator.addMethod('noplaceholder', function(value, element) {
        return value !== element.defaultValue;
    }, 'This field is required');

    $('input.clearfocus').live('focus', function() {
        if ($(this).data('initval') == null) {
            $(this).data('initval', $(this).val());
            $(this).toggleClass('focused');
        }
        initval = $(this).data('initval');
        if (initval == $(this).val()) {
            $(this).val('');
        }
    });
    $('input.clearfocus').live('blur', function() {
        initval = $(this).data('initval');
        if ($(this).val() == '') {
            $(this).val(initval);
            $(this).toggleClass('focused');
        }
    });

    $("body").on("change", "#grade-select", function(event) {
        var grade_id = $(this).val();
        $.ajax({
            type : "GET",
            url : autocomplete.site_url + "index.php",
            data : {
                action : 'subjectslist',
                grade_id : grade_id
            },
            dataType : 'json'
        }).done(function(json) {
            var html = '';
            for ( var key in json) {
                if (json.hasOwnProperty(key)) {
                    var subject = json[key];
                    html += '<li><input name="subjects[]" type="checkbox" class="subject-selector" value="' + subject.id + '" />' + subject.label + '</li>'
                }
            }
            if (html == '') {
                html = 'There are no subjects this grade';
            } else {
                html = '<ul>' + html + '</ul>';
            }
            $('#subject-select').html(html);
            $('.subjects').slideDown();
        });
    });

    $("body").on("change", ".subject-selector", function(event) {
        officebox.checkShowSubmit(".subject-selector");
        $('.medium-submit-btn').hide();
        if (officebox.showSubmit === 'true') {
            $('.medium-submit-btn').show();
        }
    });

    $('.medium-submit-btn').on('click', function(event) {
        $('#select-subjects').submit();
    });

    $('.schoolproduct-quantity').on('change', function(event) {
        var new_quantity = $(this).val();
        var product_id = $(this).data('id');
        var index = $(this).data('index');
        var product_select = $('.product-' + index);
        product_select.data('quantity', new_quantity);
        officebox.recalculatePrice(product_id, index, new_quantity, $('#add-cart-items').data('educationalinstitution_id'));
    });

    $('.subject-product-select').on('change', function(event) {
        var product_id = $(this).val();
        var index = $(this).data('index');
        if ($(this).attr('checked') == 'checked') {
            new_quantity = $(this).data('quantity');
        } else {
            new_quantity = 0;
        }
        officebox.recalculatePrice(product_id, index, new_quantity, $('#add-cart-items').data('educationalinstitution_id'));
    });

    $('.subject-product-select').on('change', function(event) {
        officebox.checkShowSubmit('.subject-product-select');
        $('.submit-schoolproduct-orderform').hide();
        if (officebox.showSubmit === 'true') {
            $('.submit-schoolproduct-orderform').show();
        }
    });

    $('.submit-schoolproduct-orderform').on('click', function(event) {
        $('#add-cart-items').submit();
    });

    $('#register-form-input-panel form').append('<span class="green-text">*HINT: School stationery list. eg. PDF, DOC, XLS</span>')

    $('#payment_method').on('change', function() {
        var type = $(this).val();
        if (type == 'EFT') {
            $('.banking-details').show();
        } else {
            $('.banking-details').hide();
        }
    });
});