﻿import * as React from 'react';
import { RouteComponentProps } from 'react-router';


const styleBoard = {
    maxWidth: '2000'
}

const styleCenter = {
    margin: 'auto'
}

const styleColumn = {
    border: 'solid',
};

const styleColumnTitle = {
    borderBottom: 'solid'
};

const styleColumnTitleText = {
    textAlign: 'center'
};

const styleColumnNext = {
    borderBottom: 'solid',
    borderTop: 'solid',
    borderRight: 'solid',
};


const styleTicket = {
    background: 'yellow',
    borderRadius: '10',
    paddingLeft: '10',
    border: 'solid'
};

const styleTicketBlocker = {
    background: 'red',
    borderRadius: '10',
    paddingLeft: '10',
    border: 'solid'
};

const styleTicketCritical = {
    background: 'orange',
    borderRadius: '10',
    paddingLeft: '10',
    border: 'solid'
};

const styleTicketMajor = {
    background: 'lightyellow',
    borderRadius: '10',
    paddingLeft: '10',
    border: 'solid'
};

const styleTicketMinor = {
    background: 'lightgreen',
    borderRadius: '10',
    paddingLeft: '10',
    border: 'solid'
};

const styleTicketTrivial = {
    background: 'lightgray',
    borderRadius: '10',
    paddingLeft: '10',
    border: 'solid'
};

const styleLink = {
    color: 'black'
};

interface Value {
    id: number;
    name: string;
    type: string;
    visibility: boolean;
    timeShown: number;
    refreshRate: number;
}

interface Board {
    id: number;
    name: string;
    fromCache: boolean;
    message: string;
    columns: BoardColumn[];
    rows: BoardRow[];
    hasChanged: boolean;
}

interface BoardColumn {
    name: string;
    issues: Issue[];
}

interface BoardRow {
    issueRow: Issue[];
}

interface Issue {
    key: string;
    fields: Fields;
}

interface Fields {
    priority: Priority;
    assignee: Assignee;
    status: Status;
    description: string;
    summary: string;
}

interface Status {
    name: string;
    id: string;
}

interface Assignee {
    displayName: string;
}

interface Priority {
    name: string;
    id: string;
}

interface FetchDataBoardList {
    boardlist: Value[];
    loading: boolean;
}

// this is a state xxxState
interface FetchDataColumns {
    boardlist: Value[];
    currentIndex: number;
    boardId: number;
    board: Board;
    boardChanged: boolean;
    loading: boolean;
}

export class BoardReader extends React.Component<RouteComponentProps<{}>, FetchDataBoardList> {     //Get all boards in list

    constructor() {
        super();
        this.state = {
            boardlist: [],
            loading: true
        };

        fetch('api/SampleData/BoardList')
            .then(response => response.json() as Promise<Value[]>)
            .then(data => {
                this.setState({ boardlist: data }, this.checkVisibility);
            });
    }

    checkVisibility() {
        var allBoards = this.state.boardlist;
        const visibleBoardList = [];

        allBoards.map((board) => {
            if (board.visibility) {
                visibleBoardList.push(board);
            }

        })

        this.setState({ boardlist: visibleBoardList, loading: false });

    }

    public render() {
        
        let boardInfo = this.state.loading
            ? <p><em>Loading...</em></p>
            : <ColumnReader boardlist={this.state.boardlist} />

        return <div style={styleBoard}>{boardInfo}</div>          
           
    }
  
} 

// test when no appSettings.json - currently creates error @boardId: this.props.boardlist[0].id
// error because generated file hass all boards with visibility false
class ColumnReader extends React.Component<{ boardlist: Value[] }, FetchDataColumns> {
    refreshTimer: number;

    constructor(props) {
        super(props);
        this.state = {
            boardlist: this.props.boardlist,
            currentIndex: 0,
            boardId: this.props.boardlist[0].id,
            board: {
                id: 0, name: "", fromCache: false, message: "", columns: [], rows: [], hasChanged: false
            },
            boardChanged: false,
            loading: true
        };

        this.nextSlide = this.nextSlide.bind(this);


        fetch('api/SampleData/BoardData?ID=' + this.state.boardId)
            .then(response => response.json() as Promise<Board>)
            .then(data => {
                this.setState({
                    board: data,
                    loading: false,
                    boardChanged: true
                }, this.RefreshRate);
            });
    }

    nextSlide() {

        var index;

        if (this.state.currentIndex == (this.state.boardlist.length - 1)) {
            index = 0;
        }
        else {
            index = this.state.currentIndex + 1;
        }


        const newState = {
            currentIndex: index,
            boardId: this.state.boardlist[index].id
        }

        this.setState(newState, this.boardLoad);
    }


    slideShow(timeShown: number) {

        setTimeout(this.nextSlide, timeShown);
    }
 
    boardLoad() {

        clearInterval(this.refreshTimer);

        fetch('api/SampleData/BoardData?ID=' + this.state.boardId)
            .then(response => response.json() as Promise<Board>)
            .then(data => {
                if (data.id == this.state.boardId) {

                    if (this.state.board.id == data.id && data.hasChanged == false) {

                        this.setState({ board: data, boardChanged: false }, this.RefreshRate);

                    }
                    else {
                        this.setState({ board: data, boardChanged: true }, this.RefreshRate);
                    }
                }

            });
    }

    RefreshRate() {
        this.refreshTimer = setInterval(
            () => this.boardLoad(),
            this.state.boardlist[this.state.currentIndex].refreshRate
        );
    }

    shouldComponentUpdate(nextProps, nextState) {

        return nextState.boardChanged;
    }

    componentWillUnmount() {
        clearInterval(this.refreshTimer);
    }

    public render() {

        if (this.state.loading) {
            return <h1>Loading...</h1>
        }
        else {

            if (this.state.board.columns.length == 0) {
                return <h1>Error loading!</h1>
            }
            else {
                return <div style={styleCenter}>
                    <BoardName
                        name={this.state.board.name}
                        fromCache={this.state.board.fromCache}
                        message={this.state.board.message}/>
                    <BoardTable board={this.state.board} />

                    {this.slideShow(this.state.boardlist[this.state.currentIndex].timeShown)}
                    
                </div>;
            }
        }

    }
}


class BoardName extends React.Component<{ name: string, fromCache: boolean, message: string }> {

    public render() {

        return <div>
            <h1><strong>Version test {this.props.name}</strong></h1>
            {this.props.fromCache ? <h4>Dati no keša</h4> : ""}<h4>{this.props.message}</h4>
        </div>
    }
}


class BoardTable extends React.Component<{ board: Board }> {
  
    public render() {
        return <tr>
            {this.props.board.columns.map((column, index) =>
                <td key={index} style={styleColumn}><Column column={column} board={this.props.board} /></td>
                )}
        </tr>
        
    }

}


class Column extends React.Component<{ column: BoardColumn , board: Board}> {
    public render() {

        let currentColumn = this.props.column;


        return <div>
            <div style={styleColumnTitle}>   <ColumnTitle name={currentColumn.name} />   </div>
            <div>   <ColumnFill column={currentColumn} />   </div>

        </div>
    }
}

class ColumnTitle extends React.Component<{name: string}> {
    public render() {

        return <h2 style={styleColumnTitleText}><strong>{this.props.name}</strong></h2>

    }
}


class ColumnFill extends React.Component<{ column: BoardColumn }> {
    public render() {        

        let SortedIssues = ColumnFill.IssuePriority(this.props.column);
    
       
        return <div>


            {
                SortedIssues.map((issue, index) =>

                    <tr key={index}> <td style={ColumnFill.PriorityColor(issue)}><Ticket issue={issue} /></td></tr>
            )
        }

        </div> 
            
      
    }

    private static PriorityColor(issue: Issue) {
        let Priority = issue.fields.priority.name;
        let Style;

        if (Priority == 'Blocker') {
            Style = styleTicketBlocker;
        }
        else if (Priority == 'Critical') {
            Style = styleTicketCritical;
        }
        else if (Priority == 'Major') {
            Style = styleTicketMajor;
        }
        else if (Priority == 'Minor') {
            Style = styleTicketMinor;
        }
        else if (Priority == 'Trivial') {
            Style = styleTicketTrivial;
        }
        else {
            Style = styleTicket;
        }


        return Style;

    }

    private static IssuePriority(column: BoardColumn) {

        const issueByPriority = [];
        let allIssues = column.issues;
      
        allIssues.forEach((issue) => {
            if (issue.fields.priority.name == 'Blocker') {
                issueByPriority.push(issue);
            }
        })

        allIssues.forEach((issue) => {
            if (issue.fields.priority.name == 'Critical') {
                issueByPriority.push(issue);
            }
        })

        allIssues.forEach((issue) => {
            if (issue.fields.priority.name == 'Major') {
                issueByPriority.push(issue);
            }
        })

        allIssues.forEach((issue) => {
            if (issue.fields.priority.name == 'Minor') {
                issueByPriority.push(issue);
            }
        })

        allIssues.forEach((issue) => {
            if (issue.fields.priority.name == 'Trivial') {
                issueByPriority.push(issue);
            }
        })

        return issueByPriority;

    }

}


class Ticket extends React.Component<{ issue: Issue }> {
    public render() {

        let currentIssue = this.props.issue;
        let linkToIssue = "https://jira.returnonintelligence.com/browse/" + currentIssue.key;

        return (
            <div>

                <div><a href={linkToIssue} target="_blank" style={styleLink}> <TicketKey keyName={currentIssue.key} /></a></div>
                <div ><TicketSummary desc={currentIssue.fields.summary} /></div>
                <div ><TicketAssignee assigneeName={Ticket.AssigneeCheck(currentIssue.fields.assignee)} /></div>
                

            </div>
        );
    }

    private static AssigneeCheck(assignee: Assignee) {

        let AssigneeName; 

        if (assignee == null) {
            AssigneeName = "None";
        }
        else {
            AssigneeName = assignee.displayName;
        }

        return AssigneeName;

    }

}


class TicketKey extends React.Component<{ keyName: string }> {
    public render() {
        return <div>
            <h4><strong>{this.props.keyName}</strong></h4>
        </div>

    }
}

class TicketSummary extends React.Component<{ desc: string }> {
    public render() {
        return <div>
            <h5>{this.props.desc}</h5>
        </div>

    }
}

class TicketAssignee extends React.Component<{ assigneeName: string }> {
    public render() {

        return <div>
            <h5>Assignee: <strong>{this.props.assigneeName}</strong></h5>
        </div>

    }
}

