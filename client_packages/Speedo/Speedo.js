let browser = mp.browsers.new("package://Speedo/index.html");
browser.active = false;
let showed = false;

let player = mp.players.local;

mp.events.add('render', () =>
{
	if (player.vehicle && player.vehicle.getPedInSeat(-1) === player.handle)
	{
		if(showed === false)
		{
			browser.execute("show();");
			showed = true;
		}

        let vel1 = player.vehicle.getSpeed() * 3.6;
        let vel = (vel1).toFixed(0);
		
		browser.execute(`speed(${vel});`);
		browser.execute(`engine(${player.vehicle.getIsEngineRunning()});`);
        browser.execute(`locked(${player.vehicle.getDoorLockStatus()});`);
	}
	else
	{
		if(showed)
		{
			browser.execute("hide();");
			showed  = false;
		}
	}
});

function playerEnterVehicleHandler(vehicle, seat) {
    let player = mp.players.local
    

    mp.events.add('render', () =>
{
        if (player.vehicle && player.vehicle.getPedInSeat(-1) === player.handle)
        {
            browser.active = true;
                let vehicle = player.vehicle;
                let speed = vehicle.getSpeed();
                let classs1 = vehicle.getClass();
                let gear =  vehicle.gear;
                speed = speed * 3.6;
                let speed1 = Math.floor(speed)

                browser.execute(`car(${gear},${speed1},${classs1});`);
        } else {
            browser.active = false;

        }

});
 }
 mp.events.add("playerEnterVehicle", playerEnterVehicleHandler);
