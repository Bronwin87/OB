/*!
 * jquery.select.js v1.0.7
 * Copyright 2013 realmdigital
 * Written by @clintlynch.
 */
var oldVal = $.fn.val;
$.fn.val = function (value) {
	var customVal = this.data('customVal');
	if(customVal && value == undefined){
		return customVal.apply(this, arguments);
	}else{
		return oldVal.apply(this, arguments);
	}
}
$(function() {
	$.widget( "custom.select", {
		options: {
			item: {},
			selectedoption: ''
	},
	_create: function() {
	    var select_input = this.element
	    var form = select_input.parents('form');
	    if(form != undefined){
	        form.bind('submit', function (){
                value = select_input.data('value');
                name = select_input.attr('name');
                select_input.removeAttr('name');
	            select_input.after('<input name="' + name + '" type="hidden" value="' + value + '" />');
	        });
	    }
		$( '#' + this.element.attr('id') + '-container' ).empty();
		this.element.attr( 'readonly', 'readonly' );
		this.element.wrap('<div id="' + this.element.attr('id') + '-container" class="selector-container" />');
		$('#' + this.element.attr('id') + '-container').width( this.element.outerWidth( true ) );
		$('body').on('mouseleave', '#' + this.element.attr('id') + '-container', function (){
			$(this).children('div').hide();
		});
		this.element.bind('click', function (){
			if($(this).next('div').css('display') == 'block'){
				$(this).next('div').hide();
			}else{
				$(this).next('div').show();
			}
		});
		this.element.css('cursor', 'pointer');
		this.element.after('<div class="scroll-pane" style="display:block; position:absolute; top:' + this.element.height() + 'px;"></div>');
		div = this.element.next('div.scroll-pane');
		div.width( this.element.outerWidth() );
		div.css( 'margin-right', this.element.css( 'margin-right' ) );
		div.css( 'margin-left', this.element.css( 'margin-left' ) );
		div.html('<ul style="float:left;"></ul>');
		ul = div.find('ul');
		ul.width( this.element.outerWidth() );
		element_id = this.element.attr('id');
		
		$.each(this.options.items, function(index, value) {
			$.each(value, function (_index, _value){
				ul.append('<li class="scroll-content-item" style="display:block;" id="' + element_id + '-option-' + _index + '" data-inputvalue="' + _index + '">' + _value + '</li>');
			});
		});

		this.element.data('customVal', function() {
			return this.data('value');
		});
		
		ul.find('li').bind('click', function (){
			$(this).parent('ul').parents('div.scroll-pane').prev('input').data( 'value', $(this).data('inputvalue') );
			$(this).parent('ul').parents('div.scroll-pane').prev('input').val($(this).html());
			$(this).parent('ul').parents('div.scroll-pane').prev('input').trigger( 'change' );
			$(this).parent('ul').parents('div.scroll-pane').hide();
		});
		div.css( "overflow", "hidden" );
		if(this.options.selectedoption != ''){
			ul.find('li#' + element_id + '-option-' + this.options.selectedoption + '').trigger('click');
		}else{
			ul.find('li:first-child').trigger('click');
		}
	}});
});