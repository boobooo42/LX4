function displayInfo(data){
    $("body").append()
}

function CreateZipfsPlot(collection) {
    $(".sidebar").show();
    $("#neural-net").empty();

    var margin = { top: 100, right: 100, bottom: 100, left: 100 },
        width = 650 - margin.left - margin.right,
        height = 650 - margin.top - margin.bottom;

    var x = d3.scale.log()
        .domain([0.1, 10])
        .range([0, width - 10]);

    var y = d3.scale.log()
        .range([height - 10, 0]);

    var color = d3.scale.category10();

    var xAxis = d3.svg.axis()
        .scale(x)
        .orient("bottom")
        .ticks(0, ".0s");

    var yAxis = d3.svg.axis()
        .scale(y)
        .orient("left")
        .ticks(0, ".0s");

    var svg = d3.select("section").append("svg")
        .attr("width", width + margin.left + margin.right)
        .attr("height", height + margin.top + margin.bottom)
      .append("g")
        .attr("transform", "translate(" + margin.left + "," + margin.top + ")");

    collection.data.Words.forEach(function (d) {
        d.Frequency = +d.Frequency;
        d.Rank = +d.Rank;
    });

    x.domain(d3.extent(collection.data.Words, function (d) { return d.Rank; })).nice();
    y.domain(d3.extent(collection.data.Words, function (d) { return d.Frequency; })).nice();

    svg.append("g")
        .attr("class", "x axis")
        .attr("transform", "translate(0," + height + ")")
        .call(xAxis)
      .append("text")
        .attr("class", "label")
        .attr("x", width/2 + 10)
        .attr("y", 50)
        .style("text-anchor", "end")
        .text("log(Rank)");

    svg.append("g")
        .attr("class", "y axis")
        .call(yAxis)
      .append("text")
        .attr("class", "label")
        .attr("transform", "rotate(-90)")
        .attr("y", -75)
        .attr("x", -(width/2) + 40)
        .attr("dy", ".71em")
        .style("text-anchor", "end")
        .text("log(Frequency)");

    svg.selectAll(".dot")
        .data(collection.data.Words)
        .enter().append("circle")
        .attr("class", "dot")
        .attr("r", 3)
        .attr("cx", function (d) { return x(d.Rank); })
        .attr("cy", function (d) { return y(d.Frequency); })
        .style("fill", function (d) { return "orange"; })
        .attr("data-toggle", "tooltip")
        .on("mouseover", function (d) {
            d3.select(this).attr("r", 6).style("fill", "black");
        })
        .on("mouseout", function (d) {
            d3.select(this).attr("r", 3).style("fill", "orange");
        })
        .on("click", function (d) {
            $('[data-toggle="tooltip"]').tooltip();
            console.log(d);
            console.log(this);
        });

    var legend = svg.selectAll(".legend")
        .data(color.domain())
      .enter().append("g")
        .attr("class", "legend")
        .attr("transform", function (d, i) { return "translate(0," + i * 20 + ")"; });

    legend.append("rect")
        .attr("x", width - 18)
        .attr("width", 18)
        .attr("height", 18)
        .style("fill", color);

    legend.append("text")
        .attr("x", width - 24)
        .attr("y", 9)
        .attr("dy", ".35em")
        .style("text-anchor", "end")
        .text(function (d) { return d; });
}