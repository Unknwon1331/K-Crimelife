mp.events.addCommand('playsound', (player, sound) => {
    player.call('sound:play', [sound])
})