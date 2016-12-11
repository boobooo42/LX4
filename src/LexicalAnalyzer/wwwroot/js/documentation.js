var documentationApp = angular.module("documentationApp", ['ngRoute']);

documentationApp.controller("DocuController", function ($scope) {
    var docu = [];
    function setInfo() {
        docu["HomeStaus"] = {};
        docu["WorkflowScraper"] = {};
        docu["WorkflowLearning"] = {};
        docu["WorkflowManageLearning"] = {};
        docu["HomeCorpus"] = {};
        docu["HomeDocumentation"] = {};
        docu["HomeLearningModel"] = {};
        docu["HomeMerkle"] = {};

        docu["HomeStaus"].title = "Status";
        docu["HomeStaus"].body = "Displays a summary of all active scrapers and learning models.";

        docu["HomeDocumentation"].title = "General Instructions";
        docu["HomeDocumentation"].body = "\nCreate corpora under Corpus";
        docu["HomeDocumentation"].body += "\nCreate a scraper under Workflows > New Scraper";
        docu["HomeDocumentation"].body += "\nManage and start scrapers under Workflows > Manage Scrapers";
        docu["HomeDocumentation"].body += "\nCreate learning model under Workflow > New Learning Model";
        docu["HomeDocumentation"].body += "\nManage and start learning model under Workflow > Manage Learning Model";
        docu["HomeDocumentation"].glossarytitle = "Glossary";
        docu["HomeDocumentation"].glossary = "\n\nCorpus: A corpus stores data which most likely came from the scraper. The main components of a corpus are the hash, which identifies the item, and the content which stores a text representation of the item.  By having a loose structure this allows for comparison of non-similar items, such as tweets and books.";
        docu["HomeDocumentation"].glossary += '\n\n\t1. Learning Model--A learning model is a method of processing a corpus. It uses a set of algorithms to analyze the content of the corpus and extract information from the raw data.  This information can be meta-data(e.g. Word frequency) or relational (e.g. "sit" is often used within a few words of "chair").  Currently the working learning models are:';
        docu["HomeDocumentation"].glossary += "\n\n\t2. The Zipf's Law learning model calculates word rank and frequency to show how they are related on a log-log graph.";
        docu["HomeDocumentation"].glossary += "\n\n\t3. Glove converts each word in a vocabulary to a vector and then uses t-SNE to visualize these word vectors as a 2D point cloud.";
        docu["HomeDocumentation"].glossary += "\n\n\t3. Glove converts each word in a vocabulary to a vector and then uses t-SNE to visualize these word vectors as a 2D point cloud.";
        docu["HomeDocumentation"].glossary += "\n\nScraper: The purpose of a scraper is to fetch large volumes of data from a website or related set of websites.  Currently, the working scrapers are:";
        docu["HomeDocumentation"].glossary += "\n\n\t1. A Project Gutenberg one which gets the text of books.";
        docu["HomeDocumentation"].glossary += "\n\n\t2. A Twitter one which gets the text of tweets.";
        docu["HomeDocumentation"].glossary += '\n\nWord Frequency: How often a given word shows up in a set of data, e.g. frequency of "very" in "stoves are very very hot" is two.';
        docu["HomeDocumentation"].glossary += '\n\nWord Rank: How often a given shows up in comparison to other words in a set of data, e.g. in the phrase "the white cats and the white dogs chase the brown dogs", "the" has rank one, "white" and "dogs" has rank two, and all other words have rank three.';
        

        docu["WorkflowLearning"].title = "New Learning Model";
        docu["WorkflowLearning"].body = "Each learning model has the following properties:";
        docu["WorkflowLearning"].body += "\n\t1. Name ";
        docu["WorkflowLearning"].body += "\n\t2. Learning model type (Zipf\'s law of Glove)";
        docu["WorkflowLearning"].body += "\n\t3. Corpus";
        docu["WorkflowLearning"].body += '\nAfter filling in the properties click "create learning model" to complete creation.';
        docu["WorkflowLearning"].body += "\n\nNote: Does start the learning model.";

        docu["WorkflowManageLearning"].title = "Manage Learning Models";
        docu["WorkflowManageLearning"].body = "Run, delete, and pause any scraper. When starting a scraper, the scraper will go until it hits a limit and will display which limit was reached.  \nNote: Progress is calculated as the download limit / download count and updates every 1.5 seconds.";

        docu["HomeCorpus"].title = "Corpus";
        docu["HomeCorpus"].body = "Corpus management. Selecting an existing corpus from the drop down menu or create a new corpus. The table shows the content of the corpus as well as meta-data about each item. Although data will be stored via scraper, if desired, data can be entered manually through Add Content. Delete a corpus by clicking Delete Corpus. This will bring up a confirmation window asking for confirmation before being deleted permanently.  \n\nNote: Individual data items cannot be edited";

        docu["HomeLearningModel"].title = "Learning Model";
        docu["HomeLearningModel"].body = "Displays the results of a learning model's analysis.  Select a scraper based on it's name.  If the learning is Zip's law it will show an exponential graph that relates word frequency with word rank.  If the learning model is Glove it will show <stuff>";
        
        docu["HomeMerkle"].title = "Merkle Tree";
        docu["HomeMerkle"].body = "<Info Placeholder>";

        docu["WorkflowScraper"].title = "Scraper";
        docu["WorkflowScraper"].body = "Each scraper has the following properties:";
        docu["WorkflowScraper"].body += "\n\t1. Name";
        docu["WorkflowScraper"].body += "\n\t2. Scraper type (Twitter or Project Gutenberg)";
        docu["WorkflowScraper"].body += "\n\t3. Corpus, which specifies where the scraped data will be stored";
        docu["WorkflowScraper"].body += "\n\t4. Time limit";
        docu["WorkflowScraper"].body += "\n\t5. Download limit";
        docu["WorkflowScraper"].body += "\nAfter filling in the properties, click create scraper to complete creation.  Note that this does not run the scraper.";
        docu["WorkflowScraper"].body += "\nNote: the scraper will stop as soon as it reaches either the time limit or download limit and both limits are mandatory.";
    }


    $scope.initDesc = function () {
        setInfo();
        var page = window.location.pathname;
        console.log(page);
        page = page.split("/").join("");
        page = page.split("LexicalAnalyzer").join("");
        console.log(page);
        console.log(docu[page]);
        if (page == "HomeDocumentation") {
            var tempArr = [];
            for (var key in docu) {
                if (key !== "HomeDocumentation")
                    tempArr.push(docu[key]);
            }
            $scope.doc = tempArr;
        }
        console.log(docu[page]);
        $scope.fields = docu[page];
    }

    $scope.initDesc();
});