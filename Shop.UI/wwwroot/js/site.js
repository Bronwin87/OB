//import { Carousel } from "../lib/bootstrap4/js/bootstrap.bundle";

// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your Javascript code.
document.addEventListener('DOMContentLoaded', () => {

    // Get all "navbar-burger" elements
    const $navbarBurgers = Array.prototype.slice.call(document.querySelectorAll('.navbar-burger'), 0);

    // Check if there are any navbar burgers
    if ($navbarBurgers.length > 0) {

        // Add a click event on each of them
        $navbarBurgers.forEach(el => {
            el.addEventListener('click', () => {

                // Get the target from the "data-target" attribute
                const target = el.dataset.target;
                const $target = document.getElementById(target);

                // Toggle the "is-active" class on both the "navbar-burger" and the "navbar-menu"
                el.classList.toggle('is-active');
                $target.classList.toggle('is-active');

            });
        });
    }

});

$(document).ready(function () {

    $('.navbar-burger').on('click', function (e) {
        e.preventDefault();

        // Get the target from the "data-target" attribute
        var target = $(this).data('target');
        var $target = $('#' + target);

        // Toggle the class on both the "navbar-burger" and the "navbar-menu"
        $(this).toggleClass('is-active');
        $target.toggleClass('is-active');
    });
});


var openProductModal = function (id) {
    document.getElementById('product-share-modal').classList.add('is-active');
    document.getElementById('product-share-modal-id').value = id;
};

var closeProductModal = function () {
    document.getElementById('product-share-modal').classList.remove('is-active');
};

var UserFavList = "";

var getUserFavList = function () {
    axios.get("/Shop/GetFavList")
        .then(res => {
            UserFavList = res.data;
        });
};

getUserFavList();

var currentDropdown = null;

var detectClickInsideDropdown = function (event) {
    if (currentDropdown !== null && currentDropdown.contains(event.target)) {
        currentDropdown.focus();
    }
    else if (currentDropdown !== null) {
        currentDropdown.classList.remove('is-active');
        currentDropdown = null;
    }
};

document.body.addEventListener('click', detectClickInsideDropdown);

var openUserFavList = function (id) {
    if (currentDropdown !== null) {
        currentDropdown.classList.remove('is-active');
    }
    currentDropdown = document.getElementById('product-dropdown-' + id);
    currentDropdown.classList.add('is-active');
    currentDropdown.focus();

    var dropdownMenu = document.getElementById('product-dropdown-menu-' + id);
    dropdownMenu.innerHTML = UserFavList;
};

$('body').on('click', '.add-to-list-link', function () {
    var form = $(this).closest('form');
    var productId = form.find('.product-id').val();
    var qty = form.find('.product-qty').val();
    var listId = $(this).attr('data-list-id');
    axios.post('/FavList/AddProduct', { listId, productId, qty })
        .then(res => {
            let btn = form.find(`#product-dropdown-${productId}`);
            btn.find('img').attr('src', '/public/like-selected-icon.png');
            btn.removeClass('is-active');
        });
});

var onAddToCartBegin = function (productId) {
    $("#AddToCart_" + productId).waitMe({
        effect: 'pulse',
        text: '',
        bg: 'rgb(238,133,52)',
        color: '#ffffff '
    });
};

var onAddToCartComplete = function (productId) {
    setTimeout(function () { $("#AddToCart_" + productId).waitMe('hide'); }, 800);
};