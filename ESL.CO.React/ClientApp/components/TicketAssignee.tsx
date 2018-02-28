import * as React from 'react';

export default class TicketAssignee extends React.Component<{ assigneeName: string }> {

    public render() {

        return <div>
            <h5>Assignee: <strong>{this.props.assigneeName}</strong></h5>
        </div>
    }
}