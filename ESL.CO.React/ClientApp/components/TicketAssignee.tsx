import * as React from 'react';

export default class TicketAssignee extends React.Component<{ assigneeName: string }> {

    public render() {

        return <div>
            <h4>{this.props.assigneeName}</h4>
        </div>

    }
}