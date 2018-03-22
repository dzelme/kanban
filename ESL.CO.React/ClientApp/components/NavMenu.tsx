import * as React from 'react';
import { Link, NavLink } from 'react-router-dom';
import { RouteComponentProps } from 'react-router';

export class NavMenu extends React.Component<RouteComponentProps<{}>, { version: String }> {

    constructor(props: RouteComponentProps<{}>, context) {
        super(props, context)
        this.state = {
            version: ''
        };

        setInterval(() => {
            fetch('api/version')
                .then(res => res.json() as Promise<string>)
                .then(data => {
                    this.setState((prevState) => {
                        if (prevState.version != '' && prevState.version != data)
                            window.location.reload();
                        return { version: data }
                    });
                })
        }, 5000); // todo: make this pretty
    }

    public render() {
        if ((this.props.location.pathname.substring(0, 6) != '/admin')
            || (sessionStorage.getItem("JwtToken") === null))
        {
            return null;
        }

        return <div className='main-nav'>
            <div className='navbar navbar-inverse'>
                <div className='navbar-header'>
                    <button type='button' className='navbar-toggle' data-toggle='collapse' data-target='.navbar-collapse'>
                        <span className='sr-only'>Toggle navigation</span>
                        <span className='icon-bar'></span>
                        <span className='icon-bar'></span>
                        <span className='icon-bar'></span>
                    </button>
                    <Link className='navbar-brand' to={'/admin'}>ESL.CO.React</Link>
                    <span className='version-aside'>{this.state.version}</span>
                </div>
                <div className='navbar-collapse collapse'>
                    <ul className='nav navbar-nav'>
                        <li className="dropdown">
                            <a className="dropdown-toggle" data-toggle="dropdown" href="#">
                                <span className='glyphicon glyphicon-cog'></span> Admin <span className="caret"></span></a>
                            <ul className={["dropdown-menu", "inverse-dropdown"].join(' ')}>
                                <li>
                                    <NavLink to={'/admin/presentations'} activeClassName='active'>
                                        <span className='glyphicon glyphicon-th-list'></span> Prezentācijas
                                    </NavLink>
                                </li>
                                <li>
                                    <NavLink to={'/admin/statistics'} activeClassName='active'>
                                        <span className='glyphicon glyphicon-stats'></span> Statistika
                                    </NavLink>
                                </li>
                            </ul>
                        </li>
                        <li>
                            <NavLink to={'/logout'} activeClassName='active'>
                                <span className='glyphicon glyphicon-log-out'></span> Atteikties
                            </NavLink>
                        </li>
                    </ul>
                </div>
            </div>
        </div>;
    }
}