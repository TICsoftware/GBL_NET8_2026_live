

document.addEventListener("DOMContentLoaded", () => {

const aboutSection =
document.querySelector(".about-section");

if (!aboutSection) return;

const collage =
aboutSection.querySelector(
".image-collage-sway"
);

if (!collage) return;

const itemEls = [
...collage.querySelectorAll(
".sync-pan-item"
)];

const leafEl =
aboutSection.querySelector(
".leaf-motion"
);

const lerp =
(a,b,t)=>
a+(b-a)*t;

let sectionHovered=false;
let hoveredItem=null;
let swayLoopRunning=false; // performance fix: track if the rAF loop is active



// IMAGE
const items =
itemEls.map((el)=>{

const inner =
el.querySelector(
".sync-pan-inner"
);

if(inner){

inner.style.position =
"absolute";

inner.style.left =
"50%";

inner.style.top =
"50%";

inner.style.width =
"125%";

inner.style.height =
"125%";

inner.style.objectFit =
"cover";

inner.style.objectPosition =
"center";

inner.style.transform =
`
translate(
-50%,
-50%
)
`;

inner.style.willChange =
"transform";

}

return{

el,

inner,

dir:
el.dataset.dir||
"rtl",

currentX:0,

targetX:0

};

});





// LEAF (OLD)
const leaf =
leafEl
?{

el:leafEl,

startX:0,
startY:0,

endX:350,
endY:150,

currentProgress:0,

targetProgress:0,

currentOpacity:0,

targetOpacity:0,

currentRotate:-14,

targetRotate:-14

}

:null;





aboutSection.addEventListener(
"mouseenter",
()=>{
sectionHovered=true;
if(!swayLoopRunning){
swayLoopRunning=true;
animate();
}
}
);

aboutSection.addEventListener(
"mouseleave",
()=>{
sectionHovered=false;
hoveredItem=null;
}
);




collage.addEventListener(
"mousemove",

(e)=>{

hoveredItem=
e.target.closest(
".sync-pan-item"
);

if(!swayLoopRunning){
swayLoopRunning=true;
animate();
}

}

);



collage.addEventListener(
"mouseleave",

()=>{

hoveredItem=
null;

}

);





function updateTargets(){

items.forEach(
(item)=>{


item.targetX=

hoveredItem===
item.el

?

(

item.dir==="rtl"

?

-22

:

22

)

:0;

});



if(leaf){

const active=

sectionHovered||

hoveredItem;


leaf.targetProgress=

active

?

1

:0;



leaf.targetOpacity=

active

?

1

:0;



leaf.targetRotate=

active

?

12

:-14;

}

}





function animate(){

updateTargets();




// IMAGE
items.forEach(
(item)=>{


item.currentX=

lerp(

item.currentX,

item.targetX,

0.045

);



if(item.inner){

item.inner.style.transform=

`
translate(
calc(
-50% +
${item.currentX}px
),

-50%
)
`;

}

});






// LEAF
if(leaf){

leaf.currentProgress=

lerp(
leaf.currentProgress,
leaf.targetProgress,
0.03
);


leaf.currentOpacity=

lerp(
leaf.currentOpacity,
leaf.targetOpacity,
0.03
);


leaf.currentRotate=

lerp(
leaf.currentRotate,
leaf.targetRotate,
0.03
);


const p=

leaf.currentProgress*

leaf.currentProgress*

(
3-
2*
leaf.currentProgress
);


const x=

leaf.startX+

(
leaf.endX-
leaf.startX
)

*
p;


const y=

leaf.startY+

(
leaf.endY-
leaf.startY
)

*
p;


leaf.el.style.opacity=

leaf.currentOpacity;


leaf.el.style.transform=

`
translate3d(
${x}px,
${y}px,
0
)

rotate(
${leaf.currentRotate}deg
)
`;

}



// performance fix: once everything has settled back to rest and
// nothing is being hovered, stop scheduling frames instead of
// looping forever in the background. mouseenter/mousemove above
// will restart the loop the moment it's needed again.
const settled = items.every(
(item)=> Math.abs(item.currentX-item.targetX) < 0.05
) && (!leaf || Math.abs(leaf.currentProgress-leaf.targetProgress) < 0.001);

if(!sectionHovered && !hoveredItem && settled){
swayLoopRunning=false;
return;
}

requestAnimationFrame(
animate
);

}



swayLoopRunning=true;
animate();

});




/* ==========================
   ABOUT IMAGE REVEAL
========================== */
gsap.registerPlugin(ScrollTrigger);



/* initial state */

gsap.set([
".collage-item-top",
".collage-turn"
], {
opacity: 0
});


/* bottom image visible */

gsap.set(".collage-item-bottom",{
opacity:1,
scale:1
});




const aboutTimeline = gsap.timeline({

scrollTrigger: {
trigger: ".about-section",
start: "top 72%",
toggleActions: "play none none none"
}

});




/* DECORATIVE IMAGE */

aboutTimeline.fromTo(

".collage-turn",

{
opacity: 0,
scale: .92
},

{
opacity: 1,
scale: 1,

duration: 1,

ease: "expo.out"
}

);





/* TOP IMAGE → RIGHT TO POSITION */

aboutTimeline.fromTo(

".collage-item-top",

{
opacity: 0,
x: 140,
scale: 1.08
},

{
opacity: 1,
x: 0,
scale: 1,

duration: 1.8,

ease: "expo.out"
},

"-=.4"

);

