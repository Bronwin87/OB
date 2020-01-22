$(function(){
	
	$('.competitor-input').bind('keyup', function(){
		total = 0;
		$('.competitor-input').each(function(){
			var value = $(this).val();
			var quantity = $(this).parent('td').parent('tr').children('td.first').children('small').html();
			quantity = parseInt( quantity );
			value = value * quantity;
			if( $.isNumeric( value ) ){
				total += parseFloat( value );
			}
		});
		total = total.toFixed( 2 );
		totals = total.split( '.' );
		$('.competitive-value').html( 'R ' + totals[0] + '<sup>' + totals[1] + '</sup>' );
		
		ourtotal = $('.our-value').html().replace ( /[^\d.]/g, '' );
		ourtotal = ourtotal / 100;
				
		totalsaving = total - ourtotal;
		totalsaving = totalsaving.toFixed( 2 );
		totalsavings = totalsaving.split( '.' );
		
		percentsaved = (total - ourtotal) * 100 / total;
		percentsaved = percentsaved.toFixed( 1 );
		
		if(totalsaving < 0){
			$('#compare-price .head p').html( '<em id="no-savings">' + 'No Saving, this doesn\'t seem right' + '</em>' + '<br />' + 'Please <a href="' + site_url + 'page/contact-us/">contact us</a> to look into this for you.' );
			$('.total-savings').html('<em id="bottom-savings-amount">R 0<sup>00</sup></em> or <em id="bottom-percent-saved">0%</em>');
		} else{
			$('#compare-price .head p').html( 'Total Savings<br/><em id="savings-amount">R ' + totalsavings[0] + '<sup>' + totalsavings[1] + '</sup></em><br/><label>Excl. VAT</label><br/>That\'s a saving of <em id="percent-saved">' + percentsaved + '%</em>' );
			$('.total-savings').html('<em id="bottom-savings-amount">R ' + totalsavings[0] + '<sup>'  + totalsavings[1] + '</sup></em> or <em id="bottom-percent-saved">' + percentsaved + '%</em>' );
		};		
	});

});