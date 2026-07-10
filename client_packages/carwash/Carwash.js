    let browser = mp.browsers.new("package://carwash/index.html");
    browser.active = false;


    mp.events.add('autoCar', (car) => {
       browser.active = true;
       mp.gui.cursor.show(true, true);    
    });


    mp.events.add('closeBrowser', () => {
       const localPlayer = mp.players.local;
       car = localPlayer.vehicle
       browser.active = false;
       mp.gui.cursor.show(false, false);
    });


    mp.events.add('cleanCar', () => {
       const localPlayer = mp.players.local;
       car = localPlayer.vehicle
       car.setDirtLevel(0);
    }); 