components.Hud.visible = true;

//components.Windows.show("Barber", JSON.stringify({"hairs":[{"Id":"1","Name":"male_Glatze","CustomisationId":"0","Price":"0","Gender":"0"},{"Id":"2","Name":"","CustomisationId":"1","Price":"0","Gender":"0"},{"Id":"3","Name":"","CustomisationId":"2","Price":"0","Gender":"0"},{"Id":"4","Name":"","CustomisationId":"3","Price":"0","Gender":"0"},{"Id":"5","Name":"","CustomisationId":"4","Price":"0","Gender":"0"},{"Id":"6","Name":"","CustomisationId":"5","Price":"0","Gender":"0"}]}) )
//components.get("Computer").app = "GpsApp"
//components.Windows.show("showLicense", '{"name": "Sezo_Escobar", "firstaid": "false", "gunlicense": "true", "driverlicense": "true", "trucklicense": "false", "motorcyclelicense": "true", "boatlicense": "true", "flyinglicensea": "false", "flyinglicenseb": "false", "taxilicense": "true", "passengertransportlicense": "true"}')
//components.NativeMenu.createMenu("Headline", "Subheadline")
//components.NativeMenu.addItem("item1")
//components.NativeMenu.show(1)
//components.NativeMenu.hide()
// components.Windows.show("Garage", '{"id": 1, "name": "Pillbox Hill"}')
//components.get("XMenu").dataItems = [{ label: "Bla", description: "Bla", icon: "http://localhost:8080/favicon.ico", id: "1", arg: ""}, { label: "Bla2", description: "Bla2", icon: "http://localhost:8080/favicon.ico", id: "2", arg: ""}, { label: "Bla3", description: "Bla3", icon: "http://localhost:8080/favicon.ico", id: "3", arg: ""}] -->

//components.Windows.show("NativeMenu", JSON.stringify({"Id":9,"Slot":3,"Weight":0,"Name":"Gusenberg","ImagePath":"Gusenberg.png","Amount":1}));
// components.Windows.show("Inventory", JSON.stringify({"responseInventory":[{"Id":"15","Slot":"3","Weight":"0","Name":"HeavyPistol","ImagePath":"HeavyPistol.png","Amount":"1"}]}));
// components.Windows.show('Inventory', '{"fillInventorySlots": "Id":9,"Slot":3,"Weight":0,"Name":"Gusenberg","ImagePath":"Gusenberg.png","Amount":1}))')

//geht
//components.Windows.show('Bank','{"name": "Sezo Escobar","money": 1110,"balance": 1110,"overviewTotal": [{"text": "Harz 4","betrag": 23030},{"text": "Entwicklersteuer","betrag": 23030}],"overviewIn": [{"betrag": 23030},{"betrag": 23030},{"betrag": 23030}],"overviewOut": [{"betrag": 23030},{"betrag": 23030},{"betrag": 223030}],"overviewInTransfer": [{"name": "Unbekannt","betrag": 23030},{"name": "Walter Hartz","betrag": 23030},{"name": "Walter Hartz","betrag": 23030}],"overviewOutTransfer": [{"name": "Simon Hooker","betrag": 23030},{"name": "Walter Hartz","betrag": 23030},{"name": "Walter Hartz","betrag": 23030}]}')
 // components.Windows.show("Login", '{"name": "Sezo_Escobar"}')
// components.Windows.show("Register")
  // components.Windows.show('Keys', '{"Spielername": "Sezo_Escobar","Haeuser":[{"name":"117","id":"0"}],"Fahrzeuge":[{"name":"Zentorno","id":"14"},{"name":"burrito3","id":"8694"},{"name":"Mule","id":"627002"}]}')
// components.Windows.show("Shop", JSON.stringify({"title": 'Shop', "id": '1', "items": [{"id": "1", "name": 'Scussi', "price": "1500", "png": "https://cdn.discordapp.com/attachments/956072301005266945/1090033897254948926/MarksmanRifle.png"},{"id": "2", "name": 'Scussi', "price": "1500", "png": "https://cdn.discordapp.com/attachments/956072301005266945/1090033897254948926/MarksmanRifle.png"}]}))
// components.Windows.show("CharacterCreator", '{"level": 10}')
// components.Windows.show("Garage", '{"id": 1, "name": "Pillbox Hill"}')
// components.Windows.show("TattooShop", '{"id": 1, "name": "Pillbox Hill", "price":"1000"}')


    window.addEventListener("message", function(event) {
    if (event.data.action == "showLoginScreen") {
        components.Windows.show("Login", event.data.data)
    }
    if (event.data.action == "showLoginError") {
        components.Login.status(event.data.state);
    }

  
    if (event.data.action == "openCharacterCreator") {
        components.Windows.show("CharacterCreator", '{"level": 10}')
    }
    if (event.data.action == "responseEmails") {
        components.EmailApp.responseEmails(event.data.data)
    }
    if (event.data.action == "responseVehicleOverviewByCategory") {
        components.FahrzeugUebersichtApp.responseVehicleOverview(event.data.data);
    }
    if (event.data.action == "responseVehicleTaxApp") {
        components.VehicleTaxApp.responseVehicleTax(event.data.data);
    }
    if (event.data.action == "showGiveMoney") {
        components.Windows.show("GiveMoney", event.data.data);
    }
    if (event.data.action == "responsePersonData") {
        components.PoliceEditPersonApp.responsePersonData(event.data.data);
    }
    if (event.data.action == "showInfocard") {
        components.Infocard.pushInfocard(event.data.content, event.data.color, event.data.imgSrc, event.data.duration, event.data.data)
    }
    
    if (event.data.action == "setGlobalNotification") {
        components.GlobalNotification.setGlobalNotification(event.data.message, event.data.duration, event.data.textcolor, event.data.icon)
    }
    if (event.data.action == "showTextInputBox") {
        components.Windows.show("TextInputBox", event.data.data)
        setTimeout(function() {
            components.TextInputBox.componentName = "Default";
        }, 50)
    }
    if (event.data.action == "showAntiAFK") {
        components.Windows.show("AntiAFK")
        setTimeout(function() {
            components.AntiAFK.power = true;
        }, 25)
    }
    if (event.data.action == "pushInfocard") {
        components.Infocard.pushInfocard()
    }
    if (event.data.action == "showWorkstation") {
        components.Windows.show("Workstation", event.data.data)
    }
    if (event.data.action == "showProgressbar") {
        components.Progressbar.setProgressbar(event.data.time)
    }
    if (event.data.action == "responseAduty") {
        components.PlayerPanel.aduty = event.data.aduty;
    }
    if (event.data.action == "responseVoiceRadioActiveType") {
        components.PlayerPanel.voiceRadioActiveType = event.data.data;
    }
    if (event.data.action == "responseMoney") {
        components.PlayerPanel.money = event.data.money
    }
    if (event.data.action == "responseVoiceRange") {
        components.PlayerPanel.voiceRange = event.data.voiceRange;
    }
    if (event.data.action == "responseRadioRange") {
        components.PlayerPanel.voiceRadio = event.data.radio;
        if (parseInt(event.data.radio) > 0) { 
            components.PlayerPanel.voiceRadioActive = true;
        } else {
            components.PlayerPanel.voiceRadioActive = false;
        }
    }
    if (event.data.action == "showBarberShop") {
        components.Windows.show("Barber", event.data.data)
    }
    if (event.data.action == "responseProfilData") {
        components.ProfileApp.setProfileData(event.data.data)
    }
    if (event.data.action == "openXMenu") {
        components.XMenu.setDataItems(event.data.data);
    }
    if (event.data.action == "responseShowSpeedo") {
        components.VehiclePanel.activeTacho = event.data.data;
    }
    if (event.data.action == "responseSpeedoData") {
        components.VehiclePanel.speed = event.data.speed;
        components.VehiclePanel.maxfuel = 100.0;
        components.VehiclePanel.fuel = event.data.fuel;
        components.VehiclePanel.engine = event.data.engine;
        components.VehiclePanel.lock = event.data.locked;
    }
    if (event.data.action == "openEjectWindow") {
        components.Windows.show("EjectWindow", event.data.data)
    }
    if (event.data.action == "responsePhoneContacts") {
        components.ContactsApp.setContactListData(event.data.data);
    }
    if (event.data.action == "responseInventory") {
        components.Windows.show("Inventory", event.data.data)
    }
    if (event.data.action == "pushPlayerNotification") {
        components.PlayerNotification.pushPlayerNotification(event.data.msg, event.data.duration, event.data.stumm, event.data.color, event.data.title, "rgba(51, 51, 51, 0.7)")
    }
    if (event.data.action == "updateCurrentContact") {
        components.ContactsOverview.contactName = event.data.name;
        components.ContactsOverview.contactNumber = event.data.number;
    }
    if (event.data.action == "responseKonversations") {
        components.MessengerListApp.responseKonversations(event.data.data);
    }
    if (event.data.action == "updateChat") {
        console.log(event.data.data)
        components.MessengerOverviewApp.updateChat(event.data.data)
    }
    if (event.data.action == "setGPSdata") {
        components.MessengerOverviewApp.setGPSdata(event.data.x, event.data.y);
    }
    if (event.data.action == "setGPSdataContactsOverview") {
        components.ContactsOverview.setGPSdata(event.data.x, event.data.y);
    }
    
    if (event.data.action == "showPhone") {
        showPhone()
    }
    if (event.data.action == "responsePhoneWallpaper") {
        components.HomeApp.responsePhoneWallpaper(event.data.wallpaper);
        console.log(event.data.wallpaper);
    }
    if (event.data.action == "responseApps") {
        components.HomeApp.responseApps(event.data.apps);
    }
    if (event.data.action == "getHomeScreen") {
        components.PhoneMainScreen.getHomeScreen()
    }
    if (event.data.action == "responseWallpaperList") {
        components.SettingsEditWallpaperApp.responseWallpaperList(event.data.data);
    }
    if (event.data.action == "responsePhoneSettings") {
        components.SettingsApp.responsePhoneSettings(event.data.flug, event.data.stumm, event.data.ablehnen)
    }
    if (event.data.action == "openGarage") {
        components.Windows.show("Garage", event.data.data)
    }
    if (event.data.action == "openLifeinvader") {
        components.Windows.show("TextInputBox", '{"textBoxObject":{"Message":"", "Callback":"LifeInvaderPurchaseAd", "Title":"1785"}}')
        setTimeout(function() {
            components.TextInputBox.componentName = "LifeInvader";
        }, 25)
    }
    if (event.data.action == "openFuelstation") {
        components.Windows.show("TextInputBox", '{"textBoxObject":{"CustomData":{"MaxLiter": 100, "FuelTime": 1000, "Price": 3}, "Callback":"fillvehicle", "Title":"' + event.data.money + '"}}')
        setTimeout(function() {
            components.TextInputBox.componentName = "Fuelstation";
            setTimeout(function() {
                components.Fuelstation.liter = event.data.liter;
            }, 10)
        }, 10)
    }
    if (event.data.action == "resetModulesBank") {
        components.Bank.resetModules()
    }

    if (event.data.action == "updateBankData") {
        console.log(event.data.data)
        components.Bank.atmData = JSON.parse(event.data.data);
    }
    if (event.data.action == "responseBankAppOverview") {
        components.BankAppOverview.responseBankAppOverview(event.data.balance, event.data.history);
        components.BankAppTransfer.bankingmaxcap = 1000000;
    }
    if (event.data.action == "responseVehicleList") {
        components.Garage.responseVehicleList(event.data.data);
    }
    if (event.data.action == "openShop") {
        components.Windows.show("Shop", event.data.data)
    }
    if (event.data.action == "openBank") {
        components.Windows.show("Bank", event.data.data)
    }
    if (event.data.action == "updateLifeInvaderAds") {
        components.LifeInvaderApp.updateLifeInvaderAds(event.data.data)
    }
    if (event.data.action == "gpsLocationsResponse") {
        components.GpsApp.gpsLocationsResponse(event.data.data);
    }
    // emotemenu
    if (event.data.action == "openEmote") {
        components.Windows.show("AnimationWheelFavoritesList", event.data.data)
    }
    if (event.data.action == "setDataItems") {
        components.AnimationWheelFavoritesList.setDataItemsAnimation(event.data.data)
        console.log("G: " + event.data.data)
    }
    if (event.data.action == "hideCallManage") {
        components.HomeApp.call = false;
        components.CallManageApp.cancelCall('[]')
        components.HomeApp.declineCall()
        components.Smartphone.change()
        components.Smartphone.show("PhoneMainScreen")
    }
    if (event.data.action == "acceptCall") {
        components.CallManageApp.acceptCall()
    }
    if (event.data.action == "showCallScreen") {
        components.Smartphone.show("CallManageApp");
    }
    if (event.data.action == "setCallData") {
        components.HomeApp.call = true;
        components.Smartphone.show("CallManageApp");
        obj = JSON.parse(event.data.data);
        if (obj.name == undefined) {
            obj.name = null
        }
        setTimeout(function() {components.CallManageApp.setCallData(JSON.stringify(obj), event.data.ringtone, event.data.stumm)}, 100);
    }
    if (event.data.action == "responseRingtoneList") {
        components.SettingsEditRingtonesApp.responseRingtoneList(event.data.data);
    }
    if (event.data.action == "responseNMenu") {
        components.NMenu.setDataItems(event.data.data);
    }
    if (event.data.action == "showComputer") {
        showComputer();
    }
    if (event.data.action == "responseComputerApps") {
        components.DesktopApp.responseComputerApps(event.data.apps)
    }
    if (event.data.action == "responseOpenServices") {
        components.ServiceListApp.responseOpenServiceList(event.data.data);
    }
    if (event.data.action == "responseAcceptedServices") {
        components.ServiceOwnApp.responseOwnServiceList(event.data.data)
    }
    if (event.data.action == "responseAllServices") {
        components.ServiceAcceptedApp.responseTeamServiceList(event.data.data);
    }
    if (event.data.action == "responseMarketPlaceCategories") {
        components.MarketplaceApp.responseMarketPlaceCategories(event.data.data);
    }
    if (event.data.action == "responseMyOffers") {
        components.MarketplaceApp.responseMyOffers(event.data.data);
    }
    if (event.data.action == "responseMarketPlaceOffers") {
        components.MarketplaceApp.responseMarketPlaceOffers(event.data.data);
    }
    if (event.data.action == "responsePlayerResults") {
        components.PoliceAktenSearchApp.responsePlayerResults(event.data.data);
    }
    if (event.data.action == "responseWantedCategories") {
        components.PoliceEditWantedsApp.responseCategories(event.data.data)
    }
    if (event.data.action == "responseCategoryReasons") {
        components.PoliceEditWantedsApp.responseCategoryReasons(event.data.data)
    }
    if (event.data.action == "closeComputer") {
        components.Computer.show(null)
    }
    if (event.data.action == "responseOpenCrimes") {
        components.PoliceEditPersonApp.responseOpenCrimes(event.data.data)
    }
    if (event.data.action == "responseJailTime") {
        components.PoliceEditPersonApp.responseJailTime(event.data.jailtime);
    }
    if (event.data.action == "responseJailCosts") {
        components.PoliceEditPersonApp.responseJailCosts(event.data.jailcost);
    }
    if (event.data.action == "showChat") {
        components.Windows.show("Chat")
    }
    if (event.data.action == "responseVehicleOverviewByPlate") {
        components.KennzeichenUebersichtApp.responsePlateOverview(event.data.data)
    }
    if (event.data.action == "responseAkte") {
        console.log(event.data.data)
        components.PoliceEditPersonApp.responseAkte(event.data.data)
        components.PoliceEditPersonApp.uU.person.change == true;
    }
    if (event.data.action == "responseLicenses") {
        components.PoliceEditPersonApp.responseLicenses(event.data.data)
    }
    if (event.data.action == "showDeathscreen") {
        components.Windows.show("Death")
    }
    if (event.data.action == "hideDeathscreen") {
        components.Death.closeDeathScreen()
    }
    if (event.data.action == "showNativeMenu") {
        components.NativeMenu.showNativeMenu(event.data.data);
    }
    if (event.data.action == "hideNativeMenu") {
        components.NativeMenu.hide()
    }
    if (event.data.action == "responseAktenList") {
        components.PoliceEditPersonApp.responseAktenList(event.data.data)
    }
    if (event.data.action == "openClothingShop") {
        components.Windows.show("ClothingShop", event.data.data)
    }
    if (event.data.action == "responseWardrobeClothes") {
        components.Wardrobe.responseWardrobeClothes(event.data.data);
    }
    if (event.data.action == "openWardrobe") {
        components.Windows.show("Wardrobe", event.data.data)
        setTimeout(function() {
            components.Wardrobe.slots = [
                { Id: 1, Name: 'Masken' },
                { Id: 4, Name: 'Hosen' },
                { Id: 6, Name: 'Schuhe' },
                { Id: 7, Name: 'Accessories' },
                { Id: 8, Name: 'Unterbekleidung' },
                { Id: 11, Name: 'Oberbekleidung' },
    
                // Props
                { Id: 'p-0', Name: 'Hut' },
                { Id: 'p-1', Name: 'Brille' },
                { Id: 'p-2', Name: 'Ohr' },
                { Id: 'p-6', Name: 'Uhr' },
                { Id: 'p-7', Name: 'Arm' },
            ]
        }, 100)

    }
    if (event.data.action == "responseClothingShopCategories") {
        components.ClothingShop.responseClothingShopCategories(event.data.data)
    }
    if (event.data.action == "responseClothingShopClothes") {
        components.ClothingShop.responseClothingShopClothes(event.data.data)
    }
    if (event.data.action == "responseCrimeProgress") {
        components.PoliceListProgressApp.responseCrimeProgress(event.data.data)
    }
    if (event.data.action == "responseStreifenData") {
        components.StreifenApp.responseStreifenData(event.data.data);
    }
    if (event.data.action == "showPersonalAusweis") {
        var data = JSON.parse(event.data.data)
        components.IdCard.updatePerso(data.firstName, data.lastName, data.birthday, data.address, data.level, data.playerId, data.casino, data.govLevel)
    } 
    if (event.data.action == "updateNutrition") {
        components.Nutrition.responseNutrition(event.data.data)
    }
    if (event.data.action == "showLizenzen") {
        var data = JSON.parse(event.data.data);
        components.Licenses.showLic(event.data.name, 0, data.weapon.value, data.drive.value, data.drive_truck.value, data.drive_bike.value, data.boat.value, 0, 0, 0, 0, 0, 0)
    }
    if (event.data.action == "responseTeamList") {
        components.TeamListApp.responseTeamMembers(event.data.data);
    }
})

document.addEventListener('keydown', (event) => {
    if (event.which === 112) {
        closePhone();
    }
    if (event.which === 114) {
        closeComputer();
    }
});

function showComputer() {
    computeropen = true;
    components.Computer.show("ComputerMainScreen");
}
function closeComputer() {
    if (computeropen == true) {
        computeropen = false;
        components.Computer.show(null)
        $.post('https://Scripts/close');
    }
}

var phoneopen = false;
var computeropen = false;


function showPhone() {
    phoneopen = true;
    components.Smartphone.show("PhoneMainScreen");
}
function closePhone() {
    if (phoneopen == true) {
        phoneopen = false; 
        components.Smartphone.invisible()
        $.post('https://Scripts/close');
    }
}

/*
        if (event.data.action == 'responseWallpaperList') {
            components.SettingsEditWallpaperApp.responseWallpaperList(JSON.stringify({
                silentv: {
                    id: 'https://i.imgur.com/tcL8txJ.png',
                    file: 'https://i.imgur.com/tcL8txJ.png',
                    name: 'SilentV'
                },
                army: {
                    id: 'https://i.imgur.com/heO7CYP.png',
                    file: 'https://i.imgur.com/heO7CYP.png',
                    name: 'Army'    
                },
                ballas: {
                    id: 'https://i.imgur.com/8u7OC0b.png',
                    file: 'https://i.imgur.com/8u7OC0b.png',
                    name: 'Ballas'
                },
                bratwa: {
                    id: 'https://i.imgur.com/OLdLskn.png',
                    file: 'https://i.imgur.com/OLdLskn.png',
                    name: 'Bratwa'
                },
                ballas: {
                    id: 'https://i.imgur.com/8u7OC0b.png',
                    file: 'https://i.imgur.com/8u7OC0b.png',
                    name: 'Ballas'
                },
                brigada: {
                    id: 'https://i.imgur.com/m4y0WKC.png',
                    file: 'https://i.imgur.com/m4y0WKC.png',
                    name: 'Brigada'
                },
                dpos: {
                    id: 'https://i.imgur.com/04oQ7j4.png',
                    file: 'https://i.imgur.com/04oQ7j4.png',
                    name: 'DPOS'
                },
                fahrschule: {
                    id: 'https://i.imgur.com/Dz8PH6u.png',
                    file: 'https://i.imgur.com/Dz8PH6u.png',
                    name: 'Fahrschule'
                },
                fib: {
                    id: 'https://i.imgur.com/R8OJiU5.png',
                    file: 'https://i.imgur.com/R8OJiU5.png',
                    name: 'FBI'
                },
                hustlers: {
                    id: 'https://i.imgur.com/R7HLxVT.jpg',
                    file: 'https://i.imgur.com/R7HLxVT.jpg',
                    name: 'Hustlers'
                },
                lcn: {
                    id: 'https://i.imgur.com/qGeyOE8.png',
                    file: 'https://i.imgur.com/qGeyOE8.png',
                    name: 'LCN'
                },
                lsc: {
                    id: 'https://i.imgur.com/dIoAWFA.jpg',
                    file: 'https://i.imgur.com/dIoAWFA.jpg',
                    name: 'LSC'
                },
                md: {
                    id: 'https://i.imgur.com/JcKQaHG.png',
                    file: 'https://i.imgur.com/JcKQaHG.png',
                    name: 'MD'
                },
                mg: {
                    id: 'https://i.imgur.com/HRwvH3v.png',
                    file: 'https://i.imgur.com/HRwvH3v.png',
                    name: 'MG13'
                },
                pier: {
                    id: 'https://i.imgur.com/vLKtJDq.jpg',
                    file: 'https://i.imgur.com/vLKtJDq.jpg',
                    name: 'Pier'
                },
                pferd: {
                    id: 'https://i.imgur.com/GpcBRrd.jpg',
                    file: 'https://i.imgur.com/GpcBRrd.jpg',
                    name: 'Pferd'
                },
            }));
        }
        */