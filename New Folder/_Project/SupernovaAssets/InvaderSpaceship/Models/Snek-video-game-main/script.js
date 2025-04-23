var p=document.getElementById("p");
var canvas=document.getElementById("canvas");
var ctx=canvas.getContext("2d");
ctx.imageSmoothingEnabled=false;
var button_data=[];
var snek_y_vector=0;
var snek_x_vector=0;
canvas.style.width=window.innerWidth
canvas.style.height=window.innerHeight
canvas.width = window.innerWidth; // Ensure canvas width matches window width
canvas.height = window.innerHeight; // Ensure canvas height matches window height
var on_title_screen=true;
//var sprite_list={
  //  snek1: document.getElementById("snek1")
//};
var snek=document.getElementById("snek");
snek.style.display="none";
snek.src="snek1.jpg";
snek.style.position="absolute";
var title_screen=document.getElementById("title screen");
var background_image=document.getElementById("background image");
background_image.style.width=window.innerWidth+"px";
background_image.style.height=window.innerHeight+"px";
var play_button=document.getElementById("play button");
play_button.style.position="absolute";
play_button.style.top=window.innerHeight/2-play_button.style.height+"px";
play_button.style.right=window.innerWidth/2-play_button.style.width+"px";
var snek_animation_frame=1;
var level_num=1;
var level=[]
var block_list=[document.getElementById("lava"),document.getElementById("water"),document.getElementById("vines"),document.getElementById("ladder"),document.getElementById("fancy_floor"),document.getElementById("bricks")];
alert(block_list[0].width/20);
noise.seed(Math.random());
function perlin_screen() {
    const scale = 0.001; // Scale factor for Perlin noise to increase detail
    for (var x2 = 0; x2 <= canvas.width; x2 += 200) {
        for (var y2 = 0; y2 <= canvas.height; y2 += 200) {
            var noiseValue = Math.abs(Math.round(noise.simplex2(x2 * scale, y2 * scale) * 255));
            if (noiseValue < 128)
            {
                ctx.drawImage(block_list[4],x2,y2,200,200);
            }
            else{
                ctx.drawImage(block_list[5],x2,y2,200,200);

            }
            
        }
    }
}

var keybinds=[
    "w","s","d","a"," "
]
play_button.addEventListener("click",()=>{
snek.style.display="block";
title_screen.style.display="none";
on_title_screen=false;
})
var x=100;
var y=100;
var sprint=0;
window.addEventListener("gamepadconnected",(event)=>{
alert("gamepad connected");
})
window.addEventListener("gamepaddisconnected",(event)=>{
    alert("gamepad disconnected");
    })
document.addEventListener("keydown",(event)=>{
    if(event.key==keybinds[0]){
        snek_y_vector=-1;
    }
    if(event.key==keybinds[1]){
        snek_y_vector=1;
    }
    if(event.key==keybinds[2]){
        snek_x_vector=1;
    }
    if(event.key==keybinds[3]){
        snek_x_vector=-1;
    }
    if(event.key==keybinds[4]){
        sprint=1;
    }
})
document.addEventListener("keyup",(event)=>{
    if(event.key==keybinds[0]){
        snek_y_vector=0;
    }
    if(event.key==keybinds[1]){
        snek_y_vector=0;
    }
    if(event.key==keybinds[2]){
        snek_x_vector=0;
    }
    if(event.key==keybinds[3]){
        snek_x_vector=0;
    }
    if(event.key==keybinds[4]){
        sprint=0;
    }
})
perlin_screen()
setInterval(function(){
const gamepad=navigator.getGamepads();
if(on_title_screen && button_data[0]==true){
    snek.style.display="block";
    title_screen.style.display="none";
    on_title_screen=false;
    
}
else{
if(gamepad[0]){
    snek_x_vector=gamepad[0].axes[0];
    snek_y_vector=gamepad[0].axes[1];
    button_data=[];
    if(gamepad[0].buttons[10].pressed){
        gamepad[0].vibrationActuator.playEffect("dual-rumble", {
            startDelay: 0,
            duration: 200,
            weakMagnitude: 1.0,
            strongMagnitude: 1.0,
          });
    }
    if(gamepad[0].buttons[7].pressed){
        gamepad[0].vibrationActuator.playEffect("dual-rumble", {
            startDelay: 0,
            duration: 10,
            weakMagnitude: 0.1,
            strongMagnitude: gamepad[0].buttons[7].value,
          });
    }
    for(i=0; i < gamepad[0].buttons.length; i++){
        button_data.push(gamepad[0].buttons[i].pressed);
    }
sprint=gamepad[0].buttons[10].touched;
}
x+=snek_x_vector*0.5*(sprint+0.5*2);
y+=snek_y_vector*0.5*(sprint+0.5*2);
}
},1)
setInterval(function(){
    snek.style.left=x+"px";
    snek.style.top=y+"px";
    if(Math.round(Math.abs(snek_x_vector)*3)/3>=0.32 ){
    snek_animation_frame+=1;
    }
    else{
        if(Math.round(Math.abs(snek_y_vector)*3)/3>=0.32){
            snek_animation_frame+=1;
        }
    }
    if(snek_x_vector>0){
        snek.style.transform="scaleX(1)";
    }
    else{
        if(snek_x_vector<0){
            snek.style.transform="scaleX(-1)";
        }
    }

snek.src="snek"+snek_animation_frame+".jpg";
if(snek_animation_frame>9){
    snek_animation_frame=1;
}
},100)