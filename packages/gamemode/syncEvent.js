mp.events.add("ChangeClothes", (player, componentId, drawable, texture) => {
	player.changeClothes(componentId, drawable, texture, false, true);
});