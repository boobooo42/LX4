// Define the merkleApp module
var merkleApp = angular.module('merkleApp', []);

merkleApp.controller('MerkleController', ['$scope', '$http', function ($scope, $http) {
    $scope.tree; 

    $scope.init = function () {
        $http({
            method: 'get',
            url: UrlContent('/api/merkle/tree'),
        })
      .success(function (response) {
          $scope.tree = response;
          console.log($scope.tree);
          BuildMerkleTree($scope.tree, false);
      })
      .error(function () {
          console.log("Failed to get Merkle tree.")
      });
    }
    $scope.init();
}]);

var fakeTree = [
      {
          "hash": "njkd347892htb43yu7ifb4u98",
          "type": "string",
          "name": "NAME",
          "isFlyweight": true,
          "children": [
            {
                "hash": "bfh4ug7368f3t7u4fbrfhfruifr9",
                "type": "string",
                "isFlyweight": true,
                "children": [
                ]
            }
          ]
      }
]

window.onload = function () {
    //BuildMerkleTree(fakeTree, false);

   // $.getJSON("http://bn-i.net/dir.php", { format: "json" })
	//	.done(function (data) { console.log("done"); })
	//	.fail(function (data) { console.log("error"); BuildMerkleTree(null, false); })
	//	.always(function (data) { console.log("complete"); BuildMerkleTree(data, false); }
	//	);

    //CreateVisualTree(merkleList);
}

//JSON.parse('[{"filename":"fn","children":[{"filename":"fnn","children":[]}]},{"filename":"fn2","children":[{"filename":"fnn2","children":[]}]}]')
var collection = "";
var cachedList;
var filteredList = [];
var searchList = [];
var searching = false;
var htmlList;
var renderTree = true;
var visualMerkleTree;

function BuildMerkleTree(jsonList, filter) {

    if (jsonList ==  undefined) {
        collection = "<h4>Merkle Tree is empty</h4>";
        AppendList(collection, false);
    }

    visualMerkleTree = jsonList;
    collection = "";

    //Cached list to search from when filtering
    if (filter == false) {
        cachedList = jsonList;
    }

    //CreateVisualTree(jsonList);

    var drawer = $("drawer");
    if (jsonList != null) {

        for (var key = 0; key < jsonList.length; key++) {
            CreateTreeBranch(jsonList[key], 0);
        }
    }

    if (searching == false) {
        htmlList = collection;
    }

    if (collection.length < 1) {
        collection = "<h4>No Results Found.</h4>";
        AppendList(collection, false);
    }
    else {
        AppendList(collection, true);
    }
};

function CreateTreeBranch(jso, level) {

    if (!searching) {
        searchList.push(jso);
    }

    if (jso.Children != undefined && jso.Children.length != 0) {
        var string = "<li><span><i class='icon-folder-open'></i> ";
    } else {
        var string = "<li><span class='empty'><i class='icon-folder-open'></i> ";
    }

    var title = jso.Type;

    if (jso.Name != undefined) {
        string += jso.Name + "</span> <a href='#' data-toggle='tooltip' data-placement='right'" + title + "'><span class='hash'>" + jso.Hash.substring(jso.Hash.length - 20, jso.Hash.length); + "</span></a>";
    }
    else {
        string += jso.Type + "</span> <a href='#' data-toggle='tooltip' data-placement='right'" + title + "'><span class='hash'>" + jso.Hash.substring(jso.Hash.length - 20, jso.Hash.length); + "</span></a>";
    }
    collection += string;
    if (jso.Children != undefined && jso.Children.length != 0) {
        collection += '<ul>';
        for (var key in jso.Children) {
            CreateTreeBranch(jso.Children[key], level + 1);
        }
        collection += "</ul>";
    }

    collection += "</li>";
}

function FilterMerkleTree(list, word) {
    for (var node = 0; node < list.length; node++) {
        if (list[node].Hash.indexOf(word) !== -1) {
            filteredList.push(list[node]);
        }
    }
}

function AppendList(string, init) {
    $(string).appendTo("#mainMerkleTree");
    if (init)
        InitDrawer();
}

function InitDrawer() {
    $(document).ready(function () {
        $('[data-toggle="tooltip"]').tooltip();
    });


    $('.tree li:has(ul)').addClass('parent_li').find(' > span').attr('title', 'Collapse this branch');
    $('.tree li.parent_li > span').on('click', function (e) {
        var children = $(this).parent('li.parent_li').find(' > ul > li');
        if (children.is(":visible")) {
            children.hide('fast');
            $(this).attr('title', 'Collapse this branch').find(' > i').addClass('icon-folder-close').removeClass('icon-folder-open');

        } else {
            children.show('fast');
            $(this).attr('title', 'Expand this branch').find(' > i').addClass('icon-folder-open').removeClass('icon-folder-close');
        }
        e.stopPropagation();
    });
}


var i;
var tree;
var svg;
var diagonal;

function CreateVisualTree(m) {
    // ************** Generate the tree diagram	 *****************
    var margin = { top: 40, right: 500, bottom: 20, left: 0 },
        width = 960 - margin.right - margin.left,
        height = 420 - margin.bottom - margin.top;

    i = 0;

    tree = d3.layout.tree()
       .size([1100, 1600]);

    diagonal = d3.svg.diagonal()
        .projection(function (d) { return [d.x, d.y]; });

    svg = d3.select("#vm").append("svg")
       .attr("width", 1600 + margin.right + margin.left)
       .attr("height", height + margin.top + margin.bottom)
     .append("g")
       .attr("transform", "translate(" + margin.left + "," + margin.top + ")");

    //$("#vm").css("position", "relative");

    root = m[0];

    update(root);
}

function update(source) {

    // Compute the new tree layout.
    var nodes = tree.nodes(root).reverse(),
        links = tree.links(nodes);

    // Normalize for fixed-depth.
    nodes.forEach(function (d) { d.y = d.depth * 50; });

    // Declare the nodes…
    var node = svg.selectAll("g.node")
        .data(nodes, function (d) { return d.id || (d.id = ++i); });

    // Enter the nodes.
    var nodeEnter = node.enter().append("g")
        .attr("class", "node")
        .attr("transform", function (d) {
            return "translate(" + d.x + "," + d.y + ")";
        });

    nodeEnter.append("circle")
      .attr("id", function (d) { return d.hash; })
        .attr("r", 10)
        .style("fill", "#fff");

    nodeEnter.append("text")
        .attr("y", function (d) {
            return d.children || d._children ? -18 : 18;
        })
        .attr("dy", ".35em")
        .attr("text-anchor", "middle")
        .text(function (d) { return d.filename; })
        .style("fill-opacity", 1);

    // Declare the links…
    var link = svg.selectAll("path.link")
        .data(links, function (d) { return d.target.id; });

    // Enter the links.
    link.enter().insert("path", "g")
        .attr("class", "link")
        .attr("d", diagonal);

}