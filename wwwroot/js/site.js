// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your Javascript code.


$(function(){
    $('.example7').jqCron({
        enabled_minute: true,
        multiple_dom: true,
        multiple_month: true,
        multiple_mins: true,
        multiple_dow: true,
        multiple_time_hours: true,
        multiple_time_minutes: true,
        default_period: 'week',
        default_value: '*/14 */2 */3 * *',
        bind_to: $('.example7-input'),
        bind_method: {
            set: function($element, value) {
                $element.val(value);
            }
        },
        no_reset_button: false,
        lang: 'en'
    });
});