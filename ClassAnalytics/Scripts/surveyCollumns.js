
google.charts.load('current', {'packages':['bar']});
google.charts.setOnLoadCallback(drawStuff);

function drawStuff() {
    var data = new google.visualization.DataTable();
    var answers = @Html.Raw(Json.Encode(Model.answers));
    var answer_array = [];
    for(i=0;i<answers.length;i++){
        answer_array[answer_array.length] = [answers[i][1],answers[i][2],answers[i][3]];
    }
    console.log(answer_array);
    data.addColumn('number', 'Question');
    data.addColumn('number', 'True Responses');
    for(i = 0; i < answer_array.length; i++)
        data.addRow([answer_array[i][0], answer_array[i][1]]);

    var options = {
        title : 'Survey Response',
        titleTextStyle:{color:'#FFF',fontSize:20},
        legend: { position: 'none' },
        colors:['#555'],
        hAxis:{
            title:'Question #',
            titleTextStyle:{color:'#FFF',fontSize:16},
            textStyle:{color:'#FFF'},
        },
        vAxis:{
            title:'True Responses',
            titleTextStyle:{color:'#FFF',fontSize:16},
            textStyle:{color:'#FFF'},
        },
        axes: {
            y: {
                all: {
                    range: {
                        max: answers[1][3],
                        min: 0
                    }
                }
            }
        },
        bar: { groupWidth: "75%" }
    };

    var chart = new google.charts.Bar(document.getElementById('top_x_div'));
    // Convert the Classic options to Material options.
    chart.draw(data, google.charts.Bar.convertOptions(options));
};
</script>