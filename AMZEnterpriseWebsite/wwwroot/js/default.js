$(function() {


    /*======================================================
                           Active AOS
       ======================================================*/

    AOS.init({
        once: true
    });

    /*======================================================
                           Active Jarallax
       ======================================================*/
    if ($.fn.jarallax) {
        $('.jarallax').jarallax({
            speed: 0.5
        });
    }


    /*======================================================
               Top Scroll Button Start
   ======================================================*/
    var browserWindow = $(window);
    browserWindow.scroll(function() {
        var GotoTop = $(window).scrollTop();
        if (GotoTop >= 400) {
            $("#btn-go-top").slideDown(500);
        } else {
            $("#btn-go-top").slideUp(500);
        }

    });
    /*======================================================
            Top Scroll Button End
======================================================*/







});

var website = (function () {

    var shaveText = function shave(selector) {
        $(selector).shave("50");
    }

    var slideShow = function slider(selector) {
        $(selector).owlCarousel({
            loop: true,
            autoplay: true,
            autoplayTimeout: 3000,
            autoplayHoverPause: true,
            margin: 0,
            rtl: true,
            nav: false,
            dots: true,
            items: 1
        });
    }

    return {
        shaveText: shaveText,
        slideShow: slideShow
    }


})();

  