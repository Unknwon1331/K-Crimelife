let KCrimelife = null;

mp.events.add('client:startdatetime', () => {
    if (KCrimelife == null) {
        KCrimelife = mp.browsers.new('package://HUD/HUD.html');
        KCrimelife.execute(`StarteUhrzeitUndDatum();`);
    }
});


mp.events.add('client:close', () => {	
	KCrimelife.destroy();
	KCrimelife = null;
});

