import * as React from 'react';
import { RouteComponentProps } from 'react-router';


const styleColumnTitle = {
    borderBottom: 'solid'
};


const styleColumn = {
    border: 'solid',
};


const styleColumnNext = {
    borderBottom: 'solid',
    borderTop: 'solid',
    borderRight: 'solid',
};


const styleTicket = {
    background: 'yellow',
    marginLeft: '30',
    borderRadius: '10',
    paddingLeft: '10',
    border: 'solid'

};

const styleTicketBlocker = {
    background: 'red',
    marginLeft: '30',
    borderRadius: '10',
    paddingLeft: '10',
    border: 'solid'

};

const styleTicketCritical = {
    background: 'orange',
    marginLeft: '30',
    borderRadius: '10',
    paddingLeft: '10',
    border: 'solid'

};

const styleTicketMajor = {
    background: 'lightyellow',
    marginLeft: '30',
    borderRadius: '10',
    paddingLeft: '10',
    border: 'solid'

};

const styleTicketMinor = {
    background: 'lightgreen',
    marginLeft: '30',
    borderRadius: '10',
    paddingLeft: '10',
    border: 'solid'

};

const styleTicketTrivial = {
    background: 'lightgray',
    marginLeft: '30',
    borderRadius: '10',
    paddingLeft: '10',
    border: 'solid'

};

const styleBoard = {
    background: 'AliceBlue'
}


interface Value {
    id: number;
    name: string;
    type: string;
}

interface Board {
    id: number;
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

interface FetchDataBoard {
    board: Board;
    loading: boolean;
}


export class ColumnReader extends React.Component<RouteComponentProps<{}>, FetchDataBoard> {

    constructor(props) {
        super(props);
        this.state = {
            board: { id: 0, columns: [], rows: [] },
            loading: true
        };

        fetch('api/SampleData/BoardData')
            .then(response => response.json() as Promise<Board>)
            .then(data => {
                this.setState({ board: data, loading: false });
            });
    }

public render() {

    return <div>

        <div>  <BoardName id={this.state.board.id} />   </div>
        <div>   <BoardTable board={this.state.board} /> </div>

        </div>;
    }
}

class BoardName extends React.Component<{ id: number }> {

    public render() {
        return <div>
            <h1>Board: #{this.props.id}</h1>
        </div>
    }
}


export class BoardTable extends React.Component<{ board: Board }> {

    public render() {
        
        return <div>
            <tr>
            {this.props.board.columns.map((column, i) =>
                    <td style={styleColumn}><Column column={column} board={this.props.board} /></td>
                )}
            </tr>
            </div>
    }

}


class Column extends React.Component<{ column: BoardColumn , board: Board}> {
    public render() {

        let currentColumn = this.props.column;


        return <div>
            <div style={styleColumnTitle}>   <ColumnTitle name = {currentColumn.name} />   </div>
            <div>   <ColumnFill column = {currentColumn} />   </div>

        </div>
    }
}

class ColumnTitle extends React.Component<{name: string}> {
    public render() {

        return <tr>
            <th>
                {this.props.name}
            </th>
        </tr>
    }
}


class ColumnFill extends React.Component<{ column: BoardColumn }> {
    public render() {        

        return <div>
            
            {this.props.column.issues.map((issue, i) =>

                <tr> <td style={ColumnFill.PriorityColor(issue)}><Ticket issue={issue} /></td></tr>
                )}
            
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

}


class Ticket extends React.Component<{ issue: Issue }> {
    public render() {

        let currentIssue = this.props.issue;

        return (
            <div>

                <div> <TicketKey keyName={currentIssue.key} /></div>
                <div ><TicketSummary desc={currentIssue.fields.summary} /></div>
                <div ><TicketPriority priority={currentIssue.fields.priority.name} /></div>
                

            </div>
        );
    }

}
/*
<div ><TicketAssignee assigneeName={Assignee} /></div>



let Assignee = "";

if (currentIssue.fields.assignee = null) {
    Assignee = "None";
}
else {
    Assignee = currentIssue.fields.assignee.displayName;
}
*/

class TicketKey extends React.Component<{ keyName: string }> {
    public render() {
        return <div>
            <h5><strong>{this.props.keyName}</strong></h5>
        </div>

    }
}

class TicketSummary extends React.Component<{ desc: string }> {
    public render() {
        return <div>
            <h5><strong>{this.props.desc}</strong></h5>
        </div>

    }
}

class TicketPriority extends React.Component<{ priority: string }> {
    public render() {

        return <div>
            <h5><strong>{this.props.priority}</strong></h5>
        </div>

    }
}
/*
class TicketAssignee extends React.Component<{ assigneeName: string }> {
    public render() {

        return <div>
            <h5>Assignee: <strong>{this.props.assigneeName}</strong></h5>
        </div>

    }
}
*/
