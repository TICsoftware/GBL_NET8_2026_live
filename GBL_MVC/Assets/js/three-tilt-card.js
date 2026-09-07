 document.addEventListener("DOMContentLoaded", () => {
    
    // Three tilt card animation js
    const tl = gsap.timeline({

        scrollTrigger:{

            trigger:".story-gallery",

            start:"top 70%",
            

            once:true,
            
            scrub: 1,

        }

    });

    tl.from(".gallery-center",{

        scale:.75,

        opacity:0,

        duration:1,

        ease:"power4.out"

    })

    .from(".gallery-left",{

        x:-250,

        y:80,

        rotate:-18,

        opacity:0,

        duration:1,

        ease:"power4.out"

    },"-=0.7")

    .from(".gallery-right",{

        x:250,

        y:80,

        rotate:18,

        opacity:0,

        duration:1,

        ease:"power4.out"

    },"-=1");

});