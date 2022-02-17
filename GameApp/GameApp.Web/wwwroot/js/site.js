
let times = 1;
let genreConteiner = $("#genre-conteiner").html()
function AddGenre(event) {
    event.preventDefault();
    genreConteiner = genreConteiner.replace('name="Genres[' + (times - 1) + ']"', 'name="Genres[' + times + ']"')
    $("#main-form").append(genreConteiner);
    times += 1;
}
let commentsPage = 0;
function PostComment(event) {
    event.preventDefault();
    $.post({
        url: 'https://localhost:44385/api/Comments/AddCommentToGame',
        contentType: 'application/json',
        data: JSON.stringify({ gameId: $("#gameId").val(), contents: $("#commentContents").val() }),
        headers:
        {
            "RequestVerificationToken": $("input[name='__RequestVerificationToken']").val()
        },
        success: AddCommentsToStart
    });
    $("#commentContents").val("");
}

function LoadMoreComments() {
    $.get({
        url: 'https://localhost:44385/api/Comments/LoadComments/' + commentsPage + '?gameId=' + $("#gameId").val(),
        success: AddComments
    });
    commentsPage++;
}
function escapeHtml(unsafe) {
    return unsafe
        .replace(/&/g, "&amp;")
        .replace(/</g, "&lt;")
        .replace(/>/g, "&gt;")
        .replace(/"/g, "&quot;")
        .replace(/'/g, "&#039;");
}
function AddComments(comments) {
    for (let i = 0; i < comments.comments.length; i++) {
        let currentComment = comments.comments[i];

        let start = `<div class="comment" id="` + currentComment.commentId + `"><div class="user-info"><ul class="d-flex nopadding"><li class="right-div" >` + escapeHtml(currentComment.username) + `</li>`;
        let loadReplies = ``;
        if (currentComment.hasComments) {
            loadReplies = `<a href="#" id='load-replies` + currentComment.commentId + `' onclick="LoadReplies(this,'` + currentComment.commentId + `')">Load More</a>`;
        }

        start = start + `<li >Posted on:` + escapeHtml(currentComment.postedOn) + `</li><li >Overall Rating:10</li></ul></div><div class="comment-text"><p >` + escapeHtml(currentComment.contents) + `</p></div><a href="#" onclick="AppendInput(this)">Reply</a> `+loadReplies+`</div>`;
        $("#comments").append($(start).append($("<div id='comment-reply'></div>").hide().append(`<textarea id="replytext` + currentComment.commentId + `" rows="2" style="width:100%" placeholder="Write your reply"></textarea>`).append(`<button class="btn btn-primary btn-lg" onclick=Reply(this,'` + currentComment.commentId +`')>Reply</button>`)));
        $("#comments").append("<hr>");
    }
}
function AddCommentsToStart(comments) {
    for (let i = 0; i < comments.comments.length; i++) {
        let currentComment = comments.comments[i];

        let start = `<div class="comment" id="` + currentComment.commentId + `"><div class="user-info"><ul class="d-flex nopadding"><li class="right-div" >` + escapeHtml(currentComment.username) + `</li>`;
        let loadReplies = ``;
        if (currentComment.hasComments) {
            loadReplies = `<a href="#" id='load-replies` + currentComment.commentId + `' onclick="LoadReplies(this,'` + currentComment.commentId + `')">Load More</a>`;
        }

        start = start + `<li >Posted on:` + escapeHtml(currentComment.postedOn) + `</li><li >Overall Rating:10</li></ul></div><div class="comment-text"><p >` + escapeHtml(currentComment.contents) + `</p></div><a href="#" onclick="AppendInput(this)">Reply</a> ` + loadReplies + `</div>`;
        $("#comments").prepend("<hr>");
        $("#comments").prepend($(start).append($("<div id='comment-reply'></div>").hide().append(`<textarea id="replytext` + currentComment.commentId + `" rows="2" style="width:100%" placeholder="Write your reply"></textarea>`).append(`<button class="btn btn-primary btn-lg" onclick=Reply(this,'` + currentComment.commentId + `')>Reply</button>`)));
        
    }
}
let showReply = false;
function AppendInput(element) {
    event.preventDefault();
    
    if (!showReply) {
        $(element.parentElement).children('#comment-reply').show()
    }
    else
    {
        $(element.parentElement).children('#comment-reply').hide()
    }
    showReply=!showReply
}
function Reply(element, id) {
    console.log($("#gameId").val())
    console.log($("#replytext" + id).val())
    console.log(id)
    $.post({
        url: 'https://localhost:44385/api/Comments/AddReply',
        contentType: 'application/json',
        data: JSON.stringify({ gameId: $("#gameId").val(), contents: $("#replytext" + id).val(), commentId: id }),
        headers:
        {
            "RequestVerificationToken": $("input[name='__RequestVerificationToken']").val()
        },
        success: function (replies) {
            
            //LoadReplies($(element.parentElement), id);
            AddReply($(element.parentElement.parentElement),replies)

        }
    });
    $("#replytext" + id).val("")
    
}

function LoadReplies(element, id) {
    event.preventDefault()
    $('#load-replies' + id).hide()
    let parrent = $(element.parentElement)
    $.get({
        url: 'https://localhost:44385/api/Comments/LoadReplies/' + id,
        success: function (replies) {
            AddReply(parrent, replies);
        }
    });
}
function AddReply(parrent,replies)
{
    for (let i = 0; i < replies.replies.length; i++) {
        let currentComment = replies.replies[i];
        let loadReplies = ``;
        if (currentComment.hasComments) {
            loadReplies = `<a href="#" id='load-replies` + currentComment.commentId + `' onclick="LoadReplies(this,'` + currentComment.commentId + `')">Load More</a>`;
        }
        let start = `<div class="littlemargin"><a>` + escapeHtml(currentComment.username) + `:</a><a>` + escapeHtml(currentComment.content) + `</a> <a href="#" onclick="AppendInput(this)">Reply</a> ` + loadReplies + `</div>`;
        parrent.append($(start).append($("<div id='comment-reply'></div>").hide().append(`<textarea id="replytext` + currentComment.commentId + `" rows="2" style="width:100%" placeholder="Write your reply"></textarea>`).append(`<button class="btn btn-primary btn-lg" onclick=Reply(this,'` + currentComment.commentId + `')>Reply</button>`)));
        parrent.append("<hr>");
    }

}
function SendFriendRequest() {
    event.preventDefault();
    $.post({
        url: 'https://localhost:44385/api/Friend/SendFirendRequest',
        contentType: 'application/json',
        data: JSON.stringify({ username: $("#friend-request-name-input").val() }),
        headers:
        {
            "RequestVerificationToken": $("input[name='__RequestVerificationToken']").val()
        },
        
    });

}

function AcceptFirendRequest(element) {
    event.preventDefault();
    $.post({
        url: 'https://localhost:44385/api/Friend/AcceptFirendRequest',
        contentType: 'application/json',
        data: JSON.stringify({ username: $(element.parentElement.parentElement).children("#request-name").text() }),
        headers:
        {
            "RequestVerificationToken": $("input[name='__RequestVerificationToken']").val()
        },

    });
}

function RejectFirendRequest(element) {
    event.preventDefault();
    $.post({
        url: 'https://localhost:44385/api/Friend/RejectFirendRequest',
        contentType: 'application/json',
        data: JSON.stringify({ username: $(element.parentElement.parentElement).children("#request-name").text() }),
        headers:
        {
            "RequestVerificationToken": $("input[name='__RequestVerificationToken']").val()
        },
        
    });
}

function Unfriend(element) {
    event.preventDefault();
    $.post({
        url: 'https://localhost:44385/api/Friend/Unfriend',
        contentType: 'application/json',
        data: JSON.stringify({ username: $(element.parentElement.parentElement).children("#friend-name").text() }),
        headers:
        {
            "RequestVerificationToken": $("input[name='__RequestVerificationToken']").val()
        },

    });
}