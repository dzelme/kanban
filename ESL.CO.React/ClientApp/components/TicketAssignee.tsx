import * as React from 'react';

export default class TicketAssignee extends React.Component<{ assigneeName: string }> {

    public render() {

        return <div>
            <h4><strong>{this.props.assigneeName}</strong></h4>
        </div>

    }
}