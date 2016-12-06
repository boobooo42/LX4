function displayInfo(data) {
    $("body").append()
}

function CreateZipfsPlot(collection) {
    $(".sidebar").show();
    $("#neural-net").empty();

    var margin = { top: 10, right: 100, bottom: 100, left: 100 },
        width = 800 - margin.left - margin.right,
        height = 700 - margin.top - margin.bottom;

    var x = d3.scale.log()
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

    collection.data.Characters.forEach(function (d) {
        d.Frequency = +d.Frequency;
        d.Rank = +d.Rank;
    });

    x.domain(d3.extent(collection.data.Words, function (d) { return d.Rank; })).nice();
    y.domain(d3.extent(collection.data.Words, function (d) { return d.Frequency; })).nice();

    console.log(collection.data.Words);
    console.log(collection.data.Characters);

    svg.append("g")
        .attr("class", "x axis")
        .attr("transform", "translate(0," + height + ")")
        .call(xAxis)
      .append("text")
        .attr("class", "label")
        .attr("x", width / 2 + 10)
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
        .attr("x", -(width / 2) + 40)
        .attr("dy", ".71em")
        .style("text-anchor", "end")
        .text("log(Frequency)");

    svg.selectAll(".wdot")
        .data(collection.data.Words)
        .enter().append("circle")
        .attr("class", "wdot")
        .attr("r", 3)
        .attr("cx", function (d) { return x(d.Rank); })
        .attr("cy", function (d) { return y(d.Frequency); })
        .style("fill", function (d) { return color("Word"); })
        .attr("data-toggle", "tooltip")
        .on("mouseover", function (d) {
            displayData(d);
            d3.select(this).attr("r", 6).style("fill", "black");
        })
        .on("mouseout", function (d) {
            d3.select(this).attr("r", 3).style("fill", color("Word"));
        })
        .on("click", function (d) {
            displayData(d);
            console.log(this);
        });

    svg.selectAll(".cdot")
       .data(collection.data.Characters)
       .enter().append("circle")
       .attr("class", "cdot")
       .attr("r", 3)
       .attr("cx", function (d) { return x(d.Rank); })
       .attr("cy", function (d) { return y(d.Frequency); })
       .style("fill", function (d) { return color("Character"); })
       .attr("data-toggle", "tooltip")
       .on("mouseover", function (d) {
           displayData(d);
           d3.select(this).attr("r", 6).style("fill", "black");
       })
       .on("mouseout", function (d) {
           d3.select(this).attr("r", 3).style("fill", color("Character"));
       })
       .on("click", function (d) {
           displayData(d);
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

function CreateZipfLawPlot(collection) {
    $(".sidebar").show();
    $("#neural-net").empty();
    var margin = { top: 50, right: 245, bottom: 50, left: 255 },
        width = 960 - margin.left - margin.right,
        height = 500 - margin.top - margin.bottom;

    var x = d3.scale.log()
        .range([0, width]);

    var y = d3.scale.log()
        .range([height, 0]);

    var color = d3.scale.category10();

    var xAxisBottom = d3.svg.axis()
        .scale(x)
        .orient("bottom")
        .ticks(0, ".0s");

    var xAxisTop = d3.svg.axis()
        .scale(x)
        .orient("top")
        .tickFormat("");

    var yAxisLeft = d3.svg.axis()
        .scale(y)
        .orient("left")
        .ticks(0, ".0s");

    var yAxisRight = d3.svg.axis()
        .scale(y)
        .orient("right")
        .tickFormat("");

    var line = d3.svg.line();

    var svg = d3.select("section").append("svg")
        .attr("width", width + margin.left + margin.right)
        .attr("height", height + margin.top + margin.bottom)
      .append("g")
        .attr("transform", "translate(" + margin.left + "," + margin.top + ")");

    svg.append("defs").append("clipPath")
        .attr("id", "clip")
      .append("rect")
        .attr("width", width)
        .attr("height", height);

    collection.data.Words.forEach(function (d) {
        d.Frequency = +d.Frequency;
        d.Rank = +d.Rank;
    });

    collection.data.Characters.forEach(function (d) {
        d.Frequency = +d.Frequency;
        d.Rank = +d.Rank;
    });

    x.domain(d3.extent(collection.data.Words, function (d) { return d.Rank; })).nice();
    y.domain(d3.extent(collection.data.Words, function (d) { return d.Frequency; })).nice();

    console.log(collection.data.Words);
    console.log(collection.data.Characters);
    
    svg.append("g")
        .attr("class", "x axis")
        .attr("transform", "translate(0," + height + ")")
        .call(xAxisBottom)
        .append("text")
            .style("text-anchor", "middle")
            .attr("transform", "translate(" + width / 2 + ",45)")
            .attr("class", "label")
            .text("log(Rank)")
            .style("font-size", "13px");


    svg.append("g")
        .attr("class", "x axis")
        .attr("transform", "translate(0, 0)")
        .call(xAxisTop);

    svg.append("g")
        .attr("class", "y axis")
        .call(yAxisLeft)
        .append("text")
            .style("text-anchor", "left")
            .attr("transform", "translate(-60, " + (height / 2) + ") rotate(-90)")
            .attr("class", "label")
            .text("log(Frequency)")
            .style("font-size", "13px");


    svg.append("g")
        .attr("class", "y axis")
        .attr("transform", "translate(" + width + ", 0)")
        .call(yAxisRight)


    svg.selectAll(".wdot")
        .data(collection.data.Words)
        .enter().append("circle")
        .attr("class", "wdot")
        .attr("r", 3)
        .attr("cx", function (d) { return x(d.Rank); })
        .attr("cy", function (d) { return y(d.Frequency); })
        .style("fill", function (d) { return color("Word"); })
        .attr("data-toggle", "tooltip")
        .on("mouseover", function (d) {
            displayData(d);
            d3.select(this).attr("r", 6).style("fill", "black");
        })
        .on("mouseout", function (d) {
            d3.select(this).attr("r", 3).style("fill", color("Word"));
        })
        .on("click", function (d) {
            displayData(d);
            console.log(this);
        });

    svg.selectAll(".cdot")
       .data(collection.data.Characters)
       .enter().append("circle")
       .attr("class", "cdot")
       .attr("r", 3)
       .attr("cx", function (d) { return x(d.Rank); })
       .attr("cy", function (d) { return y(d.Frequency); })
       .style("fill", function (d) { return color("Character"); })
       .attr("data-toggle", "tooltip")
       .on("mouseover", function (d) {
           displayData(d);
           d3.select(this).attr("r", 6).style("fill", "black");
       })
       .on("mouseout", function (d) {
           d3.select(this).attr("r", 3).style("fill", color("Character"));
       })
       .on("click", function (d) {
           displayData(d);
           console.log(this);
       });

    var legend = svg.selectAll(".legend")
        .data(color.domain())
      .enter().append("g")
        .attr("class", "legend")
        .attr("transform", function (d, i) { return "translate(0," + i * 20 + ")"; });

    legend.append("circle")
        .attr("r", 4)
        .attr("cx", width - 25)
        .attr("cy", 25)
        .style("fill", color);

    legend.append("text")
        .attr("x", width - 35)
        .attr("y", 25)
        .attr("dy", ".35em")
        .style("text-anchor", "end")
        .text(function (d) { return d; })
        .style("font-size", "11px");
}