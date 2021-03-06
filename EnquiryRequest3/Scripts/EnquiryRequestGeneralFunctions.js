﻿window.onload = function(){
    var stageFilter = document.getElementById('stageFilter');
    if (stageFilter !== null) {
        stageFilter.onchange = function () {
            document.getElementById("myForm").submit();
        }
    }
}

function SetDateTime(textBoxId) {
    var d = new Date();
    var textBox = document.getElementById(textBoxId);
    var year = '' + d.getFullYear();
    var month = '' + (d.getMonth() + 1);
    var date = '' + d.getDate();
    var hours = '' + d.getHours();
    var minutes = '' + d.getMinutes();
    var seconds = '' + d.getSeconds();
    if (month.length < 2) month = '0' + month;
    if (date.length < 2) date = '0' + date;
    if (hours.length < 2) hours = '0' + hours;
    if (minutes.length < 2) minutes = '0' + minutes;
    if (seconds.length < 2) seconds = '0' + seconds;

    textBox.value = year + "-" + month + "-" + date + "T" + hours + ":" + minutes + ":" + seconds;
}

function SetNowToElement(element)
{
    SetDateTime(element);
}

    
    