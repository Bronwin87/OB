(function($) {
$.fn.orphans = function(){
    var txt = [];
    this.each(function(){$.each(this.childNodes, function() {
        if (this.nodeType == 3 && $.trim(this.nodeValue)) txt.push(this)
    })}); 
    return $(txt);
};
$.fn.fadeToggle = function(speed, easing, callback) {
    return this.animate({opacity: 'toggle'}, speed, easing, callback);
};
$.fn.slideFadeToggle = function(speed, easing, callback) {
    return this.animate({opacity: 'toggle', height: 'toggle'}, speed, easing, callback);
};
})(jQuery);
$(function() {
    $('#content div.accordion .collapse').hide(); 
    $('#content div.accordion .expand').orphans().wrap('<a href="#" title="expand/collapse"></a>');
       
    $('div.accordion:eq(0) .expand').click(function() {
        $(this).toggleClass('open').siblings().removeClass('open').end()
        .find('ul.collapse').slideToggle().parent().siblings('li').find('ul.collapse:visible').slideToggle();
        return false;
    });
});
