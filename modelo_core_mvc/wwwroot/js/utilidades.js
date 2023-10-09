// AUTOCOMPLETE 
var $autocomplete = $('.autocomplete');
if ($autocomplete.length){
    // Opções de Autocomplete
    var dataSource = [ "c++", "java", "php", "coldfusion", "javascript", "asp", "ruby" ];
    $('.autocomplete').autocomplete({ source: dataSource });
}

//TOOLTIP
var $tooltips = $('[data-toggle=tooltip]');
if ($tooltips.length){
    $tooltips.tooltip();
}

/*//ACCORDION
var acc = document.getElementsByClassName("accord");
var i;

for (i = 0; i < acc.length; i++) {
  acc[i].addEventListener("click", function() {
    this.classList.toggle("ativo");
    var panel = this.nextElementSibling;
    if (panel.style.maxHeight){
      panel.style.maxHeight = null;
    } else {
      panel.style.maxHeight = panel.scrollHeight + "px";
    } 
  });
}*/

//GRID SETAS
var sf = {};
sf.getElements = function (id) {
  if (typeof id == "object") {
    return [id];
  } else {
    return document.querySelectorAll(id);
  }
};
sf.sortHTML = function(id, sel, sortvalue) {
  var a, b, i, ii, y, bytt, v1, v2, cc, j;
  a = sf.getElements(id);
  for (i = 0; i < a.length; i++) {
    for (j = 0; j < 2; j++) {
      cc = 0;
      y = 1;
      while (y == 1) {
        y = 0;
        b = a[i].querySelectorAll(sel);
        for (ii = 0; ii < (b.length - 1); ii++) {
          bytt = 0;
          if (sortvalue) {
            v1 = b[ii].querySelector(sortvalue).innerHTML.toLowerCase();
            v2 = b[ii + 1].querySelector(sortvalue).innerHTML.toLowerCase();
          } else {
            v1 = b[ii].innerHTML.toLowerCase();
            v2 = b[ii + 1].innerHTML.toLowerCase();
          }
          if ((j == 0 && (v1 > v2)) || (j == 1 && (v1 < v2))) {
            bytt = 1;
            break;
          }
        }
        if (bytt == 1) {
          b[ii].parentNode.insertBefore(b[ii + 1], b[ii]);
          y = 1;
          cc++;
        }
      }
      if (cc > 0) {break;}
    }
  }
};

//ACCORDION
var acc = document.getElementsByClassName("acord");
var i;

for (i = 0; i < acc.length; i++) {
  acc[i].addEventListener("click", function() {
    this.classList.toggle("ativo");
    var panel = this.nextElementSibling;
    if (panel.style.display === "block") {
      panel.style.display = "none";
    } else {
      panel.style.display = "block";
    }
  });
}

// DATAPICKER
$(function () {

$('.datePicker').datepicker(App.datePickerDefaultConfig);});

(function(w,$){
    var o = {};
    o.datePickerDefaultConfig = {
        changeMonth: true,
        changeYear: true,
        modal: true,
        dateFormat: 'dd/mm/yy',
        dayNames: ['Domingo','Segunda','Terça','Quarta','Quinta','Sexta','Sábado'],
        dayNamesMin: ['D','S','T','Q','Q','S','S','D'],
        dayNamesShort: ['Dom','Seg','Ter','Qua','Qui','Sex','Sáb','Dom'],
        monthNames: ['Janeiro','Fevereiro','Março','Abril','Maio','Junho','Julho','Agosto','Setembro','Outubro','Novembro','Dezembro'],
        monthNamesShort: ['Jan','Fev','Mar','Abr','Mai','Jun','Jul','Ago','Set','Out','Nov','Dez'],
        nextText: 'Próximo',
        prevText: 'Anterior',
        yearRange: '-110:+0' // Remover esta propriedade caso haja necessidade de anos posteriores 
    };

    w.App = o;
})(window,jQuery);

//TIMEPICKER
    if ($.fn.timepicker){
        $('.timePicker').timepicker({
            // Options
            timeSeparator: ':',           // The character to use to separate hours and minutes. (default: ':')
            showLeadingZero: true,        // Define whether or not to show a leading zero for hours < 10.(default: true)
            showMinutesLeadingZero: true, // Define whether or not to show a leading zero for minutes < 10.(default: true)
            showPeriod: false,            // Define whether or not to show AM/PM with selected time. (default: false)
            showPeriodLabels: true,       // Define if the AM/PM labels on the left are displayed. (default: true)
            periodSeparator: ' ',         // The character to use to separate the time from the time period.
           

            // trigger options
            showOn: 'focus',              // Define when the timepicker is shown.
                                          // 'focus': when the input gets focus, 'button' when the button trigger element is clicked,
                                          // 'both': when the input gets focus and when the button is clicked.
            button: null,                 // jQuery selector that acts as button trigger. ex: '#trigger_button'

            // Localization
            hourText: 'Horas',             // Define the locale text for "Hours"
            minuteText: 'Minutos',         // Define the locale text for "Minute"
            amPmText: ['', ''],       // Define the locale text for periods

            // Position
            myPosition: 'left top',       // Corner of the dialog to position, used with the jQuery UI Position utility if present.
            atPosition: 'left bottom',    // Corner of the input to position


            // custom hours and minutes
            hours: {
                starts: 0,                // First displayed hour
                ends: 23                  // Last displayed hour
            },
            minutes: {
                starts: 0,                // First displayed minute
                ends: 55,                 // Last displayed minute
                interval: 5,              // Interval of displayed minutes
                manual: []                // Optional extra entries for minutes
            },
            rows: 4,                      // Number of rows for the input tables, minimum 2, makes more sense if you use multiple of 2
            showHours: true,              // Define if the hours section is displayed or not. Set to false to get a minute only dialog
            showMinutes: true,            // Define if the minutes section is displayed or not. Set to false to get an hour only dialog
       
        });
    };

// LEGENDAS
$(document).on('click','.statusbt .open',function(){
    $(this).toggleClass('closed');
});

// TOAST

    function launch_toast() {
    var x = document.getElementById("toast")
    x.className = "show";
    setTimeout(function(){ x.className = x.className.replace("show", ""); }, 5000);
}


// MIDIAS/ACESSIBILIDADE
$(document).on('click','.midibt .open',function(){
    $(this).toggleClass('closed');
});

/*//UPLOADER
$(".uploadFileSimple").change(function() {
    $(this).prev().html($(this).val().replace(/C:\\fakepath\\/ig, ''));
});*/

/*// OPÇÃO TOAST
function toggle_visibility(id) {
var e = document.getElementById(id);
e.style.display = ((e.style.display!='block') ? 'block' : 'none');
}*/


// BOTAO COPY

new Clipboard('.copyBtn');
new Clipboard('.copyLink');
new Clipboard('.copyLinkRight');
new Clipboard('.copyText');

//TOOLTIP
$(function () {
  $('[data-toggle="tooltip"]').tooltip()
})

//BOTAO RECOLHE
$("#btn-bars").on("click", function(){

  $("header").toggleClass("open-menu");
  if ($("#btn-bars i").attr("class") == 'fa fa-angle-left'){
      $("#btn-bars i").removeClass('fa fa-angle-double-left').addClass('fa fa-angle-double-right');
  }else{
      $("#btn-bars i").removeClass('fa fa-angle-double-right').addClass('fa fa-angle-double-left'); 
  }
  
});

//FUNÇÃO BOTÃO
$(document).ready(function () {
    $('#sidebarCollapse').on('click', function () {
        $('#sidebar').toggleClass('active');
    });
});