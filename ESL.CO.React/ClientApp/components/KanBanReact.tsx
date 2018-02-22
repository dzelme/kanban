import * as React from 'react';
import { RouteComponentProps } from 'react-router';

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

const styleNextButton = {
    width: '20%',
    float: 'right',
    marginTop:'20'
}

const stylePrevButton = {
    width: '20%',
    marginTop:'20'
}


interface Value {
    id: number;
    name: string;
    type: string;
}

interface Board {
    id: number;
    name: string;
    columns: BoardColumn[];
    rows: BoardRow[];
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

interface IBoardDraw {
    boardlist: Value[];
    currentIndex: number;
}

interface FetchDataBoards {
    boardlist: Value[];
    currentIndex: number;
    boardId: number;
    loading: boolean;
}

interface FetchDataColumns {
    board: Board;
    loading: boolean;
    boardId: number;
}


class SwipeLeft extends React.Component<{ index: number, onClick: any }> {

    public render() {

        return <button style={stylePrevButton} onClick={this.props.onClick}>Previous</button>
    }

}


class SwipeRight extends React.Component<{ index: number, onClick: any }> {

    public render() {

        return <button style={styleNextButton} onClick={this.props.onClick}>Next</button>
    }

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
                this.setState({ boardlist: data, loading: false});
            });
    }


    public render() {
        

        let boardInfo = this.state.loading
            ? <p><em>Loading...</em></p>
            : <BoardDraw boardlist={this.state.boardlist} />

        return <div>{boardInfo}</div>
            
           
    }

  
}


class BoardDraw extends React.Component<{ boardlist: Value[] }, IBoardDraw> {

    constructor(props) {
        super(props);
        this.state = {
            boardlist: this.props.boardlist,
            currentIndex: 0
        };

        this.clickPrev = this.clickPrev.bind(this);
        this.clickNext = this.clickNext.bind(this);

    }


    public render() {
        

        var boardID = this.state.boardlist[this.state.currentIndex].id;

        return <div>
            <div>
            <SwipeLeft onClick={this.clickPrev} index={this.state.currentIndex} />
            <SwipeRight onClick={this.clickNext} index={this.state.currentIndex} />
            </div>

            <ColumnReader boardId={boardID} />
        </div>
    }      

    clickNext() {

        var index;

        if (this.state.currentIndex == (this.state.boardlist.length - 1)) {
            index = 0;
        }
        else {
            index = this.state.currentIndex + 1;
        }


        const newState = {
            currentIndex: index
        }

        this.setState(newState);
    }

    clickPrev() {

        var index;


        if (this.state.currentIndex == 0) {
            index = this.state.boardlist.length - 1;
        }
        else {
            index = this.state.currentIndex - 1;
        }

        const newState = {
            currentIndex: index
        }

        this.setState(newState);
    }
}


class ColumnReader extends React.Component<{ boardId: number }, FetchDataColumns> {


    constructor(props) {
        super(props);
        this.state = {
            board: {
                id: 0, name: "", columns: [], rows: []
            },
            loading: true,
            boardId: this.props.boardId
        };

        fetch('api/SampleData/BoardData?ID=' + this.state.boardId)
            .then(response => response.json() as Promise<Board>)
            .then(data => {
                this.setState({ board: data, loading: false });
            });
       

    }


    componentWillReceiveProps(nextProps) {
        if (nextProps.boardId !== this.state.boardId) {
            this.setState({ boardId: nextProps.boardId, loading: true }, this.boardIdChanged);
        }
    }


    boardIdChanged() {

        fetch('api/SampleData/BoardData?ID=' + this.state.boardId)
            .then(response => response.json() as Promise<Board>)
            .then(data => {
                this.setState({ board: data, loading: false });
            });

    }

    public render() {

        if (this.state.loading) {
            return <h1>Loading...</h1>
        }
        else {

            if (this.state.board.rows.length != 0) {

                return <div>
                    <BoardName name={this.state.board.name} />
                    <BoardTable board={this.state.board} />
                </div>;
            }
            else if (this.state.board.columns.length === 0) {
                return <h1>Error loading!</h1>
            }
            else {
                return <div>
                    <BoardName name={this.state.board.name} />
                    <BoardTable board={this.state.board} />
                </div>;
            }
        }
    }


    /*

    constructor() {
        super();
        this.state = {
            board: { id: 0, name: "", columns: [], rows: [] },
            loading: true
        };

        this.props.boardlist.map((item) =>

            fetch('api/SampleData/BoardData?ID=' + item.id)
                .then(response => response.json() as Promise<Board>)
                .then(data => {
                    this.setState({ board: data, loading: false });
                })


        );

    }

    public render() {

        return <div>

            <BoardName name={this.state.board.name} />
            <BoardTable board={this.state.board} />

        </div>;
    }
    */
}
/*
export class ColumnReader extends React.Component<RouteComponentProps<{}>, FetchDataColumns> {

    constructor() {
        super();
        this.state = {
            board: { id: 0, name: "", columns: [], rows: [] },
            loading: true
        };

        fetch('api/SampleData/BoardData')
            .then(response => response.json() as Promise<Board>)
            .then(data => { this.setState ({ board: data, loading: false }); });
    }

public render() {

    return <div>

        <BoardName name={this.state.board.name} />
        <BoardTable board={this.state.board} />

        </div>;
    }
}
*/
class BoardName extends React.Component<{ name: string }> {

    public render() {
        return <div>
            <h1><strong>{this.props.name}</strong></h1>
        </div>
    }
}


class BoardTable extends React.Component<{ board: Board }> {

    public render() {

        return <tr>
            {this.props.board.columns.map((column, index) =>
                <td style={styleColumn}><Column column={column} board={this.props.board}/></td>
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
                SortedIssues.map((issue, i) =>

                    <tr> <td style={ColumnFill.PriorityColor(issue)}><Ticket issue={issue} /></td></tr>
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

                <div><a href={linkToIssue} style={styleLink}> <TicketKey keyName={currentIssue.key} /></a></div>
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

